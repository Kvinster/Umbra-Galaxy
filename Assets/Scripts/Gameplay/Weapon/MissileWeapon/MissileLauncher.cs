using UnityEngine;

using STP.Behaviour.Core.Objects;
using STP.Gameplay.Weapon.Common;
using STP.Gameplay.Weapon.GunWeapon;
using STP.State;

namespace STP.Gameplay.Weapon.MissileWeapon {
    public class MissileLauncher : Gun {
        public override WeaponType Name => WeaponType.MissileLauncher;

        protected override string BulletType => Bullets.Missile;

        public MissileLauncher(float bulletSpeed, float reloadTime, BaseShip owner, Transform mountTrans,
            BulletCreator bulletCreator) : base(bulletSpeed, reloadTime, owner, mountTrans, bulletCreator) { }
    }
}