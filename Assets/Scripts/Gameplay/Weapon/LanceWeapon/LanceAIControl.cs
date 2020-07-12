using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.LanceWeapon {
    public class LanceAIControl : BaseWeaponControl<Lance> {
        readonly EnemyShip  _aiShip;
        EnemyState          _lastState;
        
        public LanceAIControl(Lance weapon, EnemyShip aiShip) : base(weapon) {
            _aiShip    = aiShip;
            _lastState = aiShip.State;
        }

        public override void UpdateControl(float timePassed) {
            base.UpdateControl(timePassed);
            if ( (_lastState != _aiShip.State) && (_aiShip.State == EnemyState.Patrolling) ) {
                Weapon.DropCharge();
            }
            if ( _aiShip.State == EnemyState.Chase ) {
                if ( Weapon.CurState == WeaponState.IDLE ) {
                    Weapon.PressCharging();   
                }
                if ( Weapon.CurState == WeaponState.CHARGED ) {
                    Weapon.ReleaseCharging();   
                }
            }
            _lastState = _aiShip.State;
        }
    }
}