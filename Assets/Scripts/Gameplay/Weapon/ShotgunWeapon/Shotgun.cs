using UnityEngine;

using STP.Behaviour.Core.Objects;
using STP.Gameplay.Weapon.Common;
using STP.Gameplay.Weapon.GunWeapon;

namespace STP.Gameplay.Weapon.ShotgunWeapon {
    public class Shotgun : Gun {
        const int   Barrels = 5;
        const float Spread  = 90;

        public override WeaponType Name => WeaponType.Shotgun;

        public Shotgun(bool isEnemy, float bulletSpeed, float reloadTime, BaseShip owner, Transform mountTrans,
            BulletCreator bulletCreator) : base(isEnemy, bulletSpeed, reloadTime, owner, mountTrans, bulletCreator) { }

        protected override void Fire() {
            for ( var i = -Spread / 2f; i <= Spread / 2f; i += Spread / (Barrels - 1) ) {
                BulletCreator.CreateBullet(Owner, BulletType, MountTrans.position,
                    (MountTrans.rotation * Quaternion.AngleAxis(i, Vector3.forward)) * Vector2.up, BulletSpeed);
            }
        }
    }
}