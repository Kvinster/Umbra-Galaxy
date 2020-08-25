using STP.Behaviour.Core.Objects;
using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.GunWeapon {
    public class BulletAIControl : BaseWeaponControl<Gun> {
        readonly EnemyShip     _aiShip;

        public BulletAIControl(Gun weapon, EnemyShip ship) : base(weapon) {
            _aiShip = ship;
        }

        public override void UpdateControl(float timePassed) {
            base.UpdateControl(timePassed);
            if ( _aiShip.State == EnemyState.Chase ) {
                Weapon.TryShoot();
            }
        }
    }
}