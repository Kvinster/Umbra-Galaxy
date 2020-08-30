using STP.Behaviour.Core.Objects;
using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.GunWeapon {
    public class BulletAIControl : BaseWeaponControl<Gun> {
        readonly BaseEnemyShip _aiShip;

        public BulletAIControl(Gun weapon, BaseEnemyShip ship) : base(weapon) {
            _aiShip = ship;
        }

        public override void UpdateControl(float timePassed) {
            base.UpdateControl(timePassed);
            if ( _aiShip.CanShoot ) {
                Weapon.TryShoot();
            }
        }
    }
}