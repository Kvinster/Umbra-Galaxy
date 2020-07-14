using STP.Gameplay.Weapon.Chargeable;
using STP.Gameplay.Weapon.GunWeapon;
using STP.Gameplay.Weapon.ImpulseWeapon;
using STP.Gameplay.Weapon.LanceWeapon;
using STP.Gameplay.Weapon.LaserWeapon;

namespace STP.Gameplay.Weapon.Common {
    public class WeaponCreator {
        
        public IWeaponControl GetManualWeapon(string weaponName) {
            switch ( weaponName ) {
                case Weapons.Bullets:
                    return new BulletManualControl(new Gun(400, 0.1f));
                case Weapons.Laser:
                    return new LaserManualControl(new Laser());
                case Weapons.Lance:
                    return new ChargeableManualControl(new Lance());
                case Weapons.Impulse:
                    return new ChargeableManualControl(new Impulse());
            }
            return null;
        }
        
        public IWeaponControl GetAIWeaponController(string weaponName, EnemyShip enemyShip) {
            switch ( weaponName ) {
                case Weapons.Bullets:
                    return new BulletAIControl(new Gun(400, 0.5f), enemyShip);
                case Weapons.Laser:
                    return new LaserAIControl(new Laser(), enemyShip);
                case Weapons.Lance:
                    return new ChargeableAIControl(new Lance(), enemyShip);
                case Weapons.Impulse:
                    return new ChargeableAIControl(new Impulse(), enemyShip);
            }
            return null;
        }
    }
}