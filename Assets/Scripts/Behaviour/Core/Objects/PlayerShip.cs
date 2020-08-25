using UnityEngine;

using STP.Gameplay;
using STP.Gameplay.Weapon.Common;
using STP.State.Core;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Objects {
    public class PlayerShip : BaseShip {
        public const int Hp = 99;
        const float ShipSpeed    = 250f;
        
        [NotNull] public FollowCamera Camera;
        
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
            ShipState.Position = transform.position;
            ShipState.Velocity = Rigidbody2D.velocity;
            Camera.UpdatePos(ShipState.Position);
        }
        
        public override void Init(CoreStarter starter) {
            _overlayHelper       = starter.OverlayHelper;
            WeaponControl        = starter.WeaponCreator.GetManualWeapon(WeaponType.MissileLauncher);
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