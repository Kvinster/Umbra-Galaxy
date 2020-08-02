using STP.Gameplay.Weapon.Common;
using STP.Gameplay.Weapon.GunWeapon;

namespace STP.Gameplay.Weapon.ShotgunWeapon {
    public class Shotgun : Gun {
        public override WeaponType Name => WeaponType.Shotgun;
        
        public Shotgun(float bulletSpeed, float reloadTime) : base(bulletSpeed, reloadTime) { }
    }
}