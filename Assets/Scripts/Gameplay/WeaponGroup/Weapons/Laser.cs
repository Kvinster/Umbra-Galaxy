using STP.Utils;
using UnityEngine;

namespace STP.Gameplay.WeaponGroup.Weapons {
    public class Laser : BaseWeapon {
        public void TryShoot() {
            CurState = WeaponState.FIRE;
            Debug.Log("Laser FIRE");
        }
        
        public void TryStopShoot() {
            CurState = WeaponState.CHARGED;
            Debug.Log("laser CHARGED");
        }

        protected override void AutoTransitions(float passedTime) { }
    }

}