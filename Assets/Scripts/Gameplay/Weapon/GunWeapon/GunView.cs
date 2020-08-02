using UnityEngine;

using STP.Gameplay.Weapon.Common;
using STP.State;

namespace STP.Gameplay.Weapon.GunWeapon {
    public class GunView : BaseWeaponView {
        public Transform BulletLaunchPoint;
        protected string BulletType;
        
        BaseShip      _ship;
        Gun           _weapon;
        BulletCreator _bulletCreator;
        
        void OnWeaponStateChanged(WeaponState newWeaponState) {
            if ( newWeaponState == WeaponState.FIRE ) {
                _bulletCreator.CreateBullet(_ship, BulletType, BulletLaunchPoint.position, transform.rotation * Vector2.up, _weapon.BulletSpeed);  
            }
        }

        void OnDestroy() {
            _weapon.StateChanged -= OnWeaponStateChanged;
        }

        public override void Init(CoreStarter starter, BaseShip ownerShip, BaseWeapon ownerWeapon) {
            if ( !(ownerWeapon is Gun gunWeapon) ) {
                return;
            }
            _ship                 = ownerShip;
            _weapon               = gunWeapon;
            _weapon.StateChanged += OnWeaponStateChanged;   
            _bulletCreator        = starter.BulletCreator;
            BulletType            = Bullets.PlayerBullet;
        }
    }
}