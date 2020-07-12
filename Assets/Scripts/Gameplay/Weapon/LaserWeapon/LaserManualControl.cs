using UnityEngine;

using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.LaserWeapon {
    public class LaserManualControl : BaseWeaponControl<Laser> {
        public LaserManualControl(Laser weapon) : base(weapon) { }

        public override void UpdateControl(float timePassed) {
            base.UpdateControl(timePassed);
            if ( Input.GetButton("Fire1") ) {
                Weapon.TryShoot();
            }
            else {
                if ( Weapon.CurState == WeaponState.FIRE ) {
                    Weapon.TryStopShoot();
                }
            }
        }
    }
}