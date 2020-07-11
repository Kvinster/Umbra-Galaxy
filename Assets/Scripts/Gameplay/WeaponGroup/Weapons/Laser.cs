using STP.Utils;
using UnityEngine;

namespace STP.Gameplay.WeaponGroup.Weapons {
    public class Laser : BaseWeapon {
        public override string Name => Weapons.Laser;
        
        public void TryShoot() {
            if ( CurState != WeaponState.FIRE ) {
                CurState = WeaponState.FIRE;
            }
        }
        
        public void TryStopShoot() {
            CurState = WeaponState.CHARGED;
            Debug.Log("laser CHARGED");
        }

        protected override void AutoTransitions(float passedTime) { }
    }

}