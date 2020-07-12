using STP.Gameplay.Weapon.GunWeapon;
using STP.Gameplay.Weapon.LaserWeapon;

namespace STP.Gameplay.Weapon.Common {
    public class WeaponCreator {
        
        public IWeaponControl GetManualWeapon(string weaponName) {
            switch ( weaponName ) {
                case Weapons.Bullets:
                    return new BulletManualControl(new Gun(400, 0.1f));
                case Weapons.Laser:
                    return new LaserManualControl(new Laser());
            }
            return null;
        }
        
        public IWeaponControl GetAIWeaponController(string weaponName, EnemyShip enemyShip) {
            switch ( weaponName ) {
                case Weapons.Bullets:
                    return new BulletAIControl(new Gun(400, 0.5f), enemyShip);
                case Weapons.Laser:
                    return new LaserAIControl(new Laser(), enemyShip);
            }
            return null;
        }
    }
}