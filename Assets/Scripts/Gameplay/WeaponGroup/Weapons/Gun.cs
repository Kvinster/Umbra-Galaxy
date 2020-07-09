using STP.Utils;
using UnityEngine;

namespace STP.Gameplay.WeaponGroup.Weapons {
    public class Gun : BaseWeapon {
        
        protected Timer Timer = new Timer();
        
        public int BulletSpeed => 200;

        public void TryShoot() {
            if ( CurState == WeaponState.CHARGED ) {
                CurState = WeaponState.FIRE;
                Debug.Log("FIRE");
                return;
            }
            Debug.Log("Nothing");
        }

        protected override void AutoTransitions(float passedTime) {
            switch ( CurState ) {
                case WeaponState.IDLE:
                    Timer.Start(3);
                    CurState = WeaponState.CHARGE;
                    break;
                case WeaponState.CHARGE:
                    if ( Timer.Tick(passedTime) ) {
                        Timer.Stop();
                        CurState = WeaponState.CHARGED;
                    }
                    break;
                case WeaponState.FIRE:
                    CurState = WeaponState.IDLE;
                    break;
            }
        }
    }
}