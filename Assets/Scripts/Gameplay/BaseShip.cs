using STP.State.Core;
using STP.Utils;
using STP.View;
using UnityEngine;

namespace STP.Gameplay {
    public abstract class BaseShip : CoreBehaviour, IDestructable {
        public Transform BulletLauncher;

        protected Rigidbody2D Rigidbody2D;
        protected ShipState   ShipState;

        float         _timer;

        BulletCreator _bulletCreator;
        WeaponInfo    _weapon;
        ShipInfo      _ship;
        
        bool          _inited;

        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, BulletLauncher);

        public void GetDamage(int damageAmount = 1) {
            ShipState.Hp -= damageAmount;
            if ( ShipState.Hp <= 0 ) {
                OnShipDestroy();
            }
        }

        protected void InternalInit(CoreStarter starter, WeaponInfo weapon, ShipInfo shipInfo) {
            _weapon        = weapon;
            _bulletCreator = starter.BulletCreator;
            Rigidbody2D    = GetComponent<Rigidbody2D>();
            _inited        = true;
            _ship          = shipInfo;
            ShipState     = new ShipState(_ship.Hp);
        }
        
        protected void Move(Vector2 direction) {
            var offsetVector = _ship.MaxSpeed * direction;
            MoveUtils.ApplyMovingVector(Rigidbody2D, transform, offsetVector);
        }

        protected virtual void TryShoot() {
            if ( _timer < _weapon.FirePeriod ) {
                return;
            }
            var direction = transform.rotation * Vector3.up;
            _bulletCreator.CreateBullet(_weapon.BulletType, BulletLauncher.position, direction, _weapon.BulletSpeed);
            _timer = 0f;
        }

        protected virtual void Update() {
            if ( !_inited ) {
                return;
            }
            _timer += Time.deltaTime;
        }

        protected abstract void OnShipDestroy();
    }
}