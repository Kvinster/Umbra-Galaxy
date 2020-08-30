using STP.Behaviour.Core.Objects;
using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.LaserWeapon {
    public class LaserAIControl : BaseWeaponControl<Laser> {
        readonly BaseEnemyShip _aiShip;

        public LaserAIControl(Laser weapon, BaseEnemyShip aiShip) : base(weapon) {
            _aiShip = aiShip;
        }

        public override void UpdateControl(float timePassed) {
            base.UpdateControl(timePassed);
            if ( !_aiShip.CanShoot ) {
                Weapon.TryStopShoot();
            }
            if ( _aiShip.CanShoot ) {
                Weapon.TryShoot();
            }
        }
    }
}