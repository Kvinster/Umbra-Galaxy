using UnityEngine;

using STP.Gameplay.WeaponGroup.Weapons;

namespace STP.Gameplay.WeaponGroup.Controls.AIControls {
    public class LaserAIControl : BaseWeaponControl<Laser> {
        readonly EnemyShip  _aiShip;
        EnemyState          _lastState;
        
        public LaserAIControl(Laser weapon, EnemyShip aiShip) : base(weapon) {
            _aiShip    = aiShip;
            _lastState = aiShip.State;
        }

        public override void UpdateControl(float timePassed) {
            base.UpdateControl(timePassed);
            if ( (_lastState != _aiShip.State) && (_aiShip.State == EnemyState.Patrolling) ) {
                Weapon.TryStopShoot();
            }
            if ( _aiShip.State == EnemyState.Chase ) {
                Weapon.TryShoot();
            }
            _lastState = _aiShip.State;
        }
    }
}