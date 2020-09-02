using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;
using STP.Gameplay.Weapon.GunWeapon;
using STP.State;

namespace STP.Gameplay.Weapon.MissileWeapon {
    public class MissileLauncherView : GunView {
        protected override void Init(CoreStarter starter, BaseShip ownerShip) {
            base.Init(starter, ownerShip);
            BulletType = Bullets.Missile;
        }
    }
}