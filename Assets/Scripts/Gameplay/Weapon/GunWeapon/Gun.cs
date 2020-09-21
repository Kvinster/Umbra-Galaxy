using UnityEngine;

using STP.Behaviour.Core.Objects;
using STP.Gameplay.Weapon.Common;
using STP.State;
using STP.Utils;

namespace STP.Gameplay.Weapon.GunWeapon {
    public class Gun : BaseWeapon {
        readonly Timer _timer = new Timer();

        protected readonly BaseShip      Owner;
        protected readonly Transform     MountTrans;
        protected readonly BulletCreator BulletCreator;
        protected readonly float         BulletSpeed;

        readonly float _reloadTime;

        protected virtual string BulletType => Bullets.PlayerBullet;

        public override WeaponType Name => WeaponType.Gun;

        public Gun(float bulletSpeed, float reloadTime, BaseShip owner, Transform mountTrans,
            BulletCreator bulletCreator) {
            _reloadTime    = reloadTime;

            Owner         = owner;
            MountTrans    = mountTrans;
            BulletCreator = bulletCreator;
            BulletSpeed   = bulletSpeed;

            CurState = WeaponState.Charged;
        }

        public void TryShoot() {
            if ( CurState == WeaponState.Charged ) {
                Fire();

                CurState = WeaponState.Fire;
            }
        }

        protected virtual void Fire() {
            BulletCreator.CreateBullet(Owner, BulletType, MountTrans.position, MountTrans.rotation * Vector2.up,
                BulletSpeed);
        }

        protected override void Update(float passedTime) {
            switch ( CurState ) {
                case WeaponState.Idle: {
                    _timer.Start(_reloadTime);
                    CurState = WeaponState.Charge;
                    break;
                }
                case WeaponState.Charge:
                    if ( _timer.Tick(passedTime) ) {
                        _timer.Stop();
                        CurState = WeaponState.Charged;
                    }
                    break;
                case WeaponState.Fire: {
                    CurState = WeaponState.Idle;
                    break;
                }
            }
        }
    }
}