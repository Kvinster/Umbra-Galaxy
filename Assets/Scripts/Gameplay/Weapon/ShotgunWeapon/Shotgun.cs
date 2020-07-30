using STP.Gameplay.Weapon.Common;
using STP.Gameplay.Weapon.GunWeapon;

namespace STP.Gameplay.Weapon.ShotgunWeapon {
    public class Shotgun : Gun {
        public override string Name => Weapons.ShotGun;
        
        public Shotgun(float bulletSpeed, float reloadTime) : base(bulletSpeed, reloadTime) { }
    }
}