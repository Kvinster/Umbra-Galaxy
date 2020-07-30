using UnityEngine;

using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.Chargeable {
    public class ChargeableManualControl : BaseWeaponControl<ChargeableWeapon> {
        public ChargeableManualControl(ChargeableWeapon weapon) : base(weapon) { }

        public override void UpdateControl(float timePassed) {
            base.UpdateControl(timePassed);
            if ( Input.GetButton("Fire1") ) {
                Weapon.PressCharging();
            }
            else {
                Weapon.ReleaseCharging();
            }
        }
    }
}