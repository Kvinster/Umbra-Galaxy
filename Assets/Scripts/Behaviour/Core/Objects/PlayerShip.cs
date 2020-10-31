using UnityEngine;

using STP.Behaviour.Starter;
using STP.Gameplay;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Objects {
    public class PlayerShip : BaseShip {
        const int   Hp        = 99;
        const float ShipSpeed = 500f;
        const float ShipAccel = 35f;

        [NotNull(false)] public FollowCamera Camera;
        [NotNull]        public Rigidbody2D  Rigidbody;

        CoreOverlayHelper  _overlayHelper;
        SelfDestructEngine _selfDestructEngine;

        public override ConflictSide CurrentSide => ConflictSide.Player;

        void OnTriggerEnter2D(Collider2D other) {
            var borderComp = other.gameObject.GetComponent<Border>();
            if ( borderComp ) {
                if ( _selfDestructEngine.IsActive ) {
                    _selfDestructEngine.StopSelfDestruction();
                }
                else {
                    _selfDestructEngine.StartSelfDestruction();
                }
            }
            var collectableItem = other.gameObject.GetComponent<ICollectable>();
            collectableItem?.CollectItem();
        }

        void FixedUpdate() {
            TryMove();
            UpdateWeaponControlState();
            ShipState.Position = transform.position;
            Camera.UpdatePos(ShipState.Position);
        }

        protected override void InitInternal(CoreStarter starter) {
            _overlayHelper       = starter.OverlayHelper;
            WeaponControl        = starter.WeaponCreator.GetManualWeapon(starter.PlayerController.CurWeaponType, this);
            _selfDestructEngine  = starter.CoreManager.SelfDestructEngine;
            _selfDestructEngine.Init(this);
            starter.WeaponViewCreator.AddWeaponView(this, WeaponControl?.GetControlledWeapon());
            InitShipInfo(new ShipInfo(Hp, ShipSpeed), starter.CoreManager.CorePlayerShipState);
        }

        protected override void OnShipDestroy() {
            Destroy(gameObject);
            _overlayHelper.ShowGameoverOverlay();
        }

        void TryMove() {
            var pointerOffset       = (Vector2) Input.mousePosition - new Vector2(Screen.width / 2, Screen.height / 2);
            var horizontalDirection = Input.GetAxis("Horizontal");
            var verticalDirection   = Input.GetAxis("Vertical");
            var moveDirection       = new Vector2(horizontalDirection, verticalDirection);
            if ( moveDirection != Vector2.zero ) {
                Move(moveDirection);
            }
            Rotate(pointerOffset);
        }

        void Move(Vector2 direction) {
            var accelVector = ShipAccel * direction;
            var exceededSpeed = Rigidbody.velocity.magnitude - ShipSpeed;
            if (exceededSpeed > 0) {
                var reverseVector = -Rigidbody.velocity.normalized * exceededSpeed;
                Rigidbody.AddForce(reverseVector, ForceMode2D.Impulse);
            }
            Rigidbody.AddForce(accelVector, ForceMode2D.Impulse);
        }

        void Rotate(Vector2 viewDirection) {
            MoveUtils.ApplyViewVector(Rigidbody, viewDirection);
        }
    }
}