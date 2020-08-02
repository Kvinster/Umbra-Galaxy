using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.Chargeable {
    public class ChargeableAIControl : BaseWeaponControl<ChargeableWeapon> {
        readonly EnemyShip  _aiShip;
        EnemyState          _lastState;
        
        public ChargeableAIControl(ChargeableWeapon weapon, EnemyShip aiShip) : base(weapon) {
            _aiShip    = aiShip;
            _lastState = aiShip.State;
        }

        public override void UpdateControl(float timePassed) {
            base.UpdateControl(timePassed);
            if ( (_lastState != _aiShip.State) && (_aiShip.State == EnemyState.Patrolling) ) {
                Weapon.DropCharge();
            }
            if ( _aiShip.State == EnemyState.Chase ) {
                if ( Weapon.CurState == WeaponState.Idle ) {
                    Weapon.PressCharging();   
                }
                if ( Weapon.CurState == WeaponState.Charged ) {
                    Weapon.ReleaseCharging();   
                }
            }
            _lastState = _aiShip.State;
        }
    }
}