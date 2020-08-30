using STP.Behaviour.Core.Objects;
using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.Chargeable {
    public class ChargeableAIControl : BaseWeaponControl<ChargeableWeapon> {
        readonly BaseEnemyShip _aiShip;

        public ChargeableAIControl(ChargeableWeapon weapon, BaseEnemyShip aiShip) : base(weapon) {
            _aiShip = aiShip;
        }

        public override void UpdateControl(float timePassed) {
            base.UpdateControl(timePassed);
            if ( !_aiShip.CanShoot ) {
                Weapon.DropCharge();
            }
            if ( _aiShip.CanShoot ) {
                if ( Weapon.CurState == WeaponState.Idle ) {
                    Weapon.PressCharging();
                }
                if ( Weapon.CurState == WeaponState.Charged ) {
                    Weapon.ReleaseCharging();
                }
            }
        }
    }
}