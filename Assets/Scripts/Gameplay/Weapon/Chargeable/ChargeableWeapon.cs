using UnityEngine;

using STP.Gameplay.Weapon.Common;
using STP.Utils;

namespace STP.Gameplay.Weapon.Chargeable {
    public abstract class ChargeableWeapon : BaseWeapon  {
        public   abstract  float Damage       { get; }
        protected abstract float ChargingTime { get; }
        
        protected readonly Timer Timer = new Timer();
        
        bool _timerWorking;

        public void PressCharging() {
            if ( CurState == WeaponState.IDLE ) {
                CurState = WeaponState.CHARGE;
            }
        }

        public void DropCharge() {
            CurState = WeaponState.IDLE;
            _timerWorking = false;
        }

        public void ReleaseCharging() {
            if ( CurState == WeaponState.CHARGE ) {
                DropCharge();
            }

            if ( CurState == WeaponState.CHARGED ) {
                Debug.Log($"FIRE");
                CurState = WeaponState.FIRE;
            }
        }
        
        protected override void AutoTransitions(float passedTime) {
            switch ( CurState ) {
                case WeaponState.CHARGE:
                    Debug.Log($"TIME {Timer.LeftTime}");
                    if ( !_timerWorking ) {
                        Timer.Start(ChargingTime);    
                        _timerWorking = true;
                    }
                    if ( Timer.Tick(passedTime) ) {
                        Timer.Stop();
                        _timerWorking = false;
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