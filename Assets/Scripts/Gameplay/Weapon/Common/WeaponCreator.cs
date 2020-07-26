using STP.Gameplay.Weapon.Chargeable;
using STP.Gameplay.Weapon.GunWeapon;
using STP.Gameplay.Weapon.ImpulseWeapon;
using STP.Gameplay.Weapon.LanceWeapon;
using STP.Gameplay.Weapon.LaserWeapon;
using STP.Gameplay.Weapon.ShotgunWeapon;

namespace STP.Gameplay.Weapon.Common {
    public class WeaponCreator {
        
        public IWeaponControl GetManualWeapon(string weaponName) {
            switch ( weaponName ) {
                case Weapons.Gun:
                    return new BulletManualControl(new Gun(400, 0.1f));
                case Weapons.Laser:
                    return new LaserManualControl(new Laser());
                case Weapons.Lance:
                    return new ChargeableManualControl(new Lance());
                case Weapons.Impulse:
                    return new ChargeableManualControl(new Impulse());
                case Weapons.ShotGun:
                    return new BulletManualControl(new Shotgun(400, 0.5f));
            }
            return null;
        }
        
        public IWeaponControl GetAIWeaponController(string weaponName, EnemyShip enemyShip) {
            switch ( weaponName ) {
                case Weapons.Gun:
                    return new BulletAIControl(new Gun(400, 0.5f), enemyShip);
                case Weapons.Laser:
                    return new LaserAIControl(new Laser(), enemyShip);
                case Weapons.Lance:
                    return new ChargeableAIControl(new Lance(), enemyShip);
                case Weapons.Impulse:
                    return new ChargeableAIControl(new Impulse(), enemyShip);
                case Weapons.ShotGun:
                    return new BulletAIControl(new Shotgun(400, 0.5f), enemyShip);
            }
            return null;
        }
    }
}