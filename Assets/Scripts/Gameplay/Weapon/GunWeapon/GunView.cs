using UnityEngine;

using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;
using STP.Gameplay.Weapon.Common;
using STP.State;

namespace STP.Gameplay.Weapon.GunWeapon {
    public class GunView : BaseWeaponView<Gun> {
        public Transform BulletLaunchPoint;
        protected string BulletType;

        BaseShip      _ship;
        BulletCreator _bulletCreator;

        void OnWeaponStateChanged(WeaponState newWeaponState) {
            if ( newWeaponState == WeaponState.Fire ) {
                _bulletCreator.CreateBullet(_ship, BulletType, BulletLaunchPoint.position,
                    transform.rotation * Vector2.up, Weapon.BulletSpeed);
            }
        }

        void OnDestroy() {
            Weapon.StateChanged -= OnWeaponStateChanged;
        }

        protected override void Init(CoreStarter starter, BaseShip ownerShip) {
            _ship                = ownerShip;
            _bulletCreator       = starter.BulletCreator;
            BulletType           = Bullets.PlayerBullet;
            Weapon.StateChanged += OnWeaponStateChanged;
        }
    }
}
