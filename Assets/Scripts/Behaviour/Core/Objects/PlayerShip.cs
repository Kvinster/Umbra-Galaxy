using UnityEngine;

using STP.Behaviour.Starter;
using STP.Gameplay;
using STP.Gameplay.Weapon.Common;
using STP.State.Core;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Objects {
    public class PlayerShip : BaseShip {
        const int   Hp           = 99;
        const float ShipSpeed    = 250f;

        [NotNull] public FollowCamera Camera;

        PlayerShipState    _shipState;
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

        void Update() {
            TryMove();
            _selfDestructEngine.UpdateSelfDestructionTimers(Time.deltaTime);
            UpdateWeaponControlState();
            _shipState.Position = transform.position;
            _shipState.Velocity = Rigidbody2D.velocity;
            Camera.UpdatePos(_shipState.Position);
        }

        public override void Init(CoreStarter starter) {
            _shipState                       = starter.CoreManager.PlayerShipState;
            _shipState.StateChangedManually += OnChangedState;
            _overlayHelper                   = starter.OverlayHelper;
            WeaponControl                    = starter.WeaponCreator.GetManualWeapon(WeaponType.MissileLauncher);
            _selfDestructEngine              = starter.CoreManager.SelfDestructEngine;
            _selfDestructEngine.Init(this);
            starter.WeaponViewCreator.AddWeaponView(this, WeaponControl?.GetControlledWeapon());
            InitShipInfo(new ShipInfo(Hp, ShipSpeed));
        }

        protected override void OnShipDestroy() {
            Destroy(gameObject);
            _overlayHelper.ShowGameoverOverlay();
        }

        void OnChangedState() {
            transform.position   = _shipState.Position;
            Rigidbody2D.velocity = _shipState.Velocity;
            Camera.UpdatePos(_shipState.Position);
        }

        void TryMove() {
            var pointerOffset       = (Vector2) Input.mousePosition - new Vector2(Screen.width/2, Screen.height/2);
            var horizontalDirection = Input.GetAxis("Horizontal");
            var verticalDirection   = Input.GetAxis("Vertical");
            var moveDirection       = new Vector2(horizontalDirection, verticalDirection);
            if ( moveDirection != Vector2.zero ) {
                Move(moveDirection);
            }
            Rotate(pointerOffset);
        }
    }
}