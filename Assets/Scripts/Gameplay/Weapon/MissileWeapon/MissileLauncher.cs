﻿using STP.Gameplay.Weapon.Common;
using STP.Gameplay.Weapon.GunWeapon;

namespace STP.Gameplay.Weapon.MissileWeapon {
    public class MissileLauncher : Gun {
        public override WeaponType Name => WeaponType.MissileLauncher;
        
        public MissileLauncher(float bulletSpeed, float reloadTime) : base(bulletSpeed, reloadTime) { }
    }
}