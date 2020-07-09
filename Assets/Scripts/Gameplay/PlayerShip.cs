using UnityEngine;

using STP.Gameplay.WeaponGroup;
using STP.Gameplay.WeaponGroup.Weapons;
using STP.State.Core;
using STP.Utils;

namespace STP.Gameplay {
    public class PlayerShip : BaseShip {
        const int   Hp           = 4;
        const float ShipSpeed    = 250f;
        
        public FollowCamera Camera;
        
        PlayerShipState   _shipState;
        CoreOverlayHelper _overlayHelper;
        
        protected override void CheckDescription() {
            base.CheckDescription();
            ProblemChecker.LogErrorIfNullOrEmpty(this, Camera);
        }

        public override void Init(CoreStarter starter) {
            _shipState                       = starter.CoreManager.PlayerShipState;
            _shipState.StateChangedManually += OnChangedState;
            _overlayHelper                   = starter.OverlayHelper;
            WeaponControl                    = starter.WeaponCreator.GetManualWeapon(Weapons.Laser);
            InternalInit(starter, new ShipInfo(Hp, ShipSpeed));
        }
        
        protected override void Update() {
            base.Update();
            TryMove();
            UpdateWeaponControlState();
            _shipState.Position = transform.position;
            _shipState.Velocity = Rigidbody2D.velocity;
            Camera.UpdatePos(_shipState.Position);
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
            var pointerDirection    = (Vector2) (Camera.Camera.ScreenToWorldPoint(Input.mousePosition) - transform.position);
            var horizontalDirection = Input.GetAxis("Horizontal");
            var verticalDirection   = Input.GetAxis("Vertical");
            var moveDirection       = new Vector2(horizontalDirection, verticalDirection);
            if ( moveDirection != Vector2.zero ) {
                Move(moveDirection);
            }
            if ( pointerDirection != Vector2.zero ) {
                Rotate(pointerDirection);
            }
        }
    }
}