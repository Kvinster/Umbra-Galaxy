using UnityEngine;

using STP.State;
using STP.State.Core;
using STP.Utils;
using STP.View;

namespace STP.Gameplay {
    public class PlayerShip : BaseShip {
        const int   Hp           = 4;
        const float ShipSpeed    = 250f;
        const int   BulletSpeed  = 400;
        const float BulletPeriod = 0.2f;
        
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
            InternalInit(starter, new WeaponInfo(BulletNames.PlayerBullet, BulletPeriod, BulletSpeed), new ShipInfo(Hp, ShipSpeed));
        }
        
        protected override void Update() {
            base.Update();
            TryMove();
            TryShoot();
            _shipState.Position = transform.position;
            _shipState.Velocity = Rigidbody2D.velocity;
            Camera.UpdatePos(_shipState.Position);
        }

        protected override void OnShipDestroy() {
            Destroy(gameObject);
            _overlayHelper.TryShowGameoverOverlay();
        }
        
        protected override void TryShoot() {
            if ( Input.GetButton("Fire1") ) {
                base.TryShoot();   
            }
        }

        void OnChangedState() {
            transform.position   = _shipState.Position;
            Rigidbody2D.velocity = _shipState.Velocity;
            Camera.UpdatePos(_shipState.Position);
        }
        
        void TryMove() {
            var horizontalDirection = Input.GetAxis("Horizontal");
            var verticalDirection   = Input.GetAxis("Vertical");
            if ( Mathf.Abs(horizontalDirection) > float.Epsilon || Mathf.Abs(verticalDirection) > float.Epsilon ) {
                Move(new Vector2(horizontalDirection, verticalDirection));
            }
        }
    }
}