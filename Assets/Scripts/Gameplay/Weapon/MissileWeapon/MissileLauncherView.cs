using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;
using STP.Gameplay.Weapon.Common;
using STP.Gameplay.Weapon.GunWeapon;
using STP.State;

namespace STP.Gameplay.Weapon.MissileWeapon {
    public class MissileLauncherView : GunView {
        public override void Init(CoreStarter starter, BaseShip ownerShip, BaseWeapon ownerWeapon) {
            base.Init(starter, ownerShip, ownerWeapon);
            BulletType = Bullets.Missile;
        }
    }
}