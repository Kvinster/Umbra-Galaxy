using UnityEngine;

using STP.Gameplay.Weapon.Common;
using STP.Utils;

namespace STP.Gameplay.Weapon.LanceWeapon {
    public class Lance : BaseWeapon  {
        public const float Damage = 5f;
        
        const float ChargingTime  = 1f;
        
        readonly Timer _timer = new Timer();
        
        bool _timerWorking;
        public override string Name => Weapons.Lance;

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
                CurState = WeaponState.FIRE;
            }
        }
        
        protected override void AutoTransitions(float passedTime) {
            switch ( CurState ) {
                case WeaponState.CHARGE:
                    Debug.Log($"TIME {_timer.LeftTime}");
                    if ( !_timerWorking ) {
                        _timer.Start(ChargingTime);    
                        _timerWorking = true;
                    }
                    if ( _timer.Tick(passedTime) ) {
                        _timer.Stop();
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