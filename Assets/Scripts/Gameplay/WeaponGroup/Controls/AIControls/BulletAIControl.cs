using STP.Gameplay.WeaponGroup.Weapons;

namespace STP.Gameplay.WeaponGroup.Controls.AIControls {
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