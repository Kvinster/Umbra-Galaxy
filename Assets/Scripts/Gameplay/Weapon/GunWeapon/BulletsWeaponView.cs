using STP.Gameplay.Weapon.Common;
using STP.Gameplay.Weapon.GunWeapon;
using UnityEngine;
using STP.State;

namespace STP.Gameplay.WeaponViews {
    public class BulletsWeaponView : BaseWeaponView {
        public Transform BulletLaunchPoint;
        
        BaseShip      _ship;
        Gun           _weapon;
        BulletCreator _bulletCreator;
        string        _bulletType;
        
        bool _inited;
        
        void OnWeaponStateChanged(WeaponState newWeaponState) {
            if ( newWeaponState == WeaponState.FIRE ) {
                _bulletCreator.CreateBullet(_bulletType, BulletLaunchPoint.position, _ship.transform.rotation * Vector2.up, _weapon.BulletSpeed);  
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
            _bulletType           = Bullets.PlayerBullet;
            _inited               = true;
        }
    }
}