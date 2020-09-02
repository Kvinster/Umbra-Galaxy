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
            if ( CurState == WeaponState.Idle ) {
                CurState = WeaponState.Charge;
            }
        }

        public void DropCharge() {
            CurState = WeaponState.Idle;
            _timerWorking = false;
        }

        public virtual void ReleaseCharging() {
            if ( CurState == WeaponState.Charge ) {
                DropCharge();
            }

            if ( CurState == WeaponState.Charged ) {
                Debug.Log($"FIRE");
                CurState = WeaponState.Fire;
            }
        }

        protected override void Update(float passedTime) {
            switch ( CurState ) {
                case WeaponState.Charge: {
                    Debug.Log($"TIME {Timer.TimeLeft}");
                    if ( !_timerWorking ) {
                        Timer.Start(ChargingTime);
                        _timerWorking = true;
                    }
                    if ( Timer.Tick(passedTime) ) {
                        Timer.Stop();
                        _timerWorking = false;
                        CurState = WeaponState.Charged;
                    }
                    break;
                }
                case WeaponState.Fire: {
                    CurState = WeaponState.Idle;
                    break;
                }
            }
        }
    }
}