using UnityEngine;

using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.LanceWeapon {
    public class LanceManualControl : BaseWeaponControl<Lance> {
        public LanceManualControl(Lance weapon) : base(weapon) { }

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