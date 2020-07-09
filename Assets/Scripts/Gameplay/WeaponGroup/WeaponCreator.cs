using STP.Gameplay.WeaponGroup.Controls;
using STP.Gameplay.WeaponGroup.Controls.AIControls;
using STP.Gameplay.WeaponGroup.Controls.PlayerControls;
using STP.Gameplay.WeaponGroup.Weapons;

namespace STP.Gameplay.WeaponGroup {
    public class WeaponCreator {
        BulletCreator _bulletCreator;
        
        public WeaponCreator(BulletCreator creator) {
            _bulletCreator = creator;
        }
        
        public IWeaponControl GetManualWeapon(string weaponName) {
            switch ( weaponName ) {
                case Weapons.Weapons.Bullets:
                    return new BulletManualControl(new Gun(), _bulletCreator);
                case Weapons.Weapons.Laser:
                    return new LaserManualControl(new Laser());
            }
            return null;
        }
        
        public IWeaponControl GetAIWeaponController(string weaponName, EnemyShip enemyShip) {
            switch ( weaponName ) {
                case Weapons.Weapons.Bullets:
                    return new BulletAIControl(new Gun(), enemyShip);
                case Weapons.Weapons.Laser:
                    return new LaserAIControl(new Laser(), enemyShip);
            }
            return null;
        }
    }
}