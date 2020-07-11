﻿using STP.Utils;
using UnityEngine;

namespace STP.Gameplay.WeaponGroup.Weapons {
    public class Gun : BaseWeapon {
        readonly Timer _timer = new Timer();
        
        float _reloadTime;
        
        public float BulletSpeed {get;}
        
        public override string Name => Weapons.Bullets;
        
        public Gun(float bulletSpeed, float reloadTime) {
            BulletSpeed = bulletSpeed;
            _reloadTime = reloadTime;
        }
        
        public void TryShoot() {
            if ( CurState == WeaponState.CHARGED ) {
                CurState = WeaponState.FIRE;
            }
        }
        
        protected override void AutoTransitions(float passedTime) {
            switch ( CurState ) {
                case WeaponState.IDLE:
                    _timer.Start(_reloadTime);
                    CurState = WeaponState.CHARGE;
                    break;
                case WeaponState.CHARGE:
                    if ( _timer.Tick(passedTime) ) {
                        _timer.Stop();
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