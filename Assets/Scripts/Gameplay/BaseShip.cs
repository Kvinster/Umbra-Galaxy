using STP.State.Core;
using STP.Utils;
using STP.View;
using UnityEngine;

namespace STP.Gameplay {
    public abstract class BaseShip : CoreBehaviour, IDestructable {
        public Transform BulletLauncher;
        public HpBar     HpBar;

        protected Rigidbody2D Rigidbody2D;
        protected ShipState   ShipState;

        float         _timer;

        BulletCreator _bulletCreator;
        WeaponInfo    _weapon;
        ShipInfo      _shipInfo;
        
        bool          _inited;

        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, BulletLauncher);

        public void GetDamage(int damageAmount = 1) {
            ShipState.Hp -= damageAmount;
            if ( ShipState.Hp <= 0 ) {
                OnShipDestroy();
            }
            HpBar.UpdateBar(((float)ShipState.Hp) / _shipInfo.Hp);
        }

        protected void InternalInit(CoreStarter starter, WeaponInfo weapon, ShipInfo shipInfo) {
            _weapon        = weapon;
            _bulletCreator = starter.BulletCreator;
            Rigidbody2D    = GetComponent<Rigidbody2D>();
            _inited        = true;
            _shipInfo      = shipInfo;
            ShipState      = new ShipState(_shipInfo.Hp);
            HpBar.Init();
            HpBar.UpdateBar(1f);
        }
        
        protected void Move(Vector2 direction) { 
            var offsetVector = _shipInfo.MaxSpeed * direction;
            MoveUtils.ApplyMovingVector(Rigidbody2D, offsetVector);
        }
        
        protected void Rotate(Vector2 viewDirection) {
            MoveUtils.ApplyViewVector(transform, viewDirection);
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