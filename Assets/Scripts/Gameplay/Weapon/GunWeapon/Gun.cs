using STP.Gameplay.Weapon.Common;
using STP.Utils;

namespace STP.Gameplay.Weapon.GunWeapon {
    public class Gun : BaseWeapon {
        readonly Timer _timer = new Timer();
        
        float _reloadTime;
        
        public float BulletSpeed {get;}
        
        public override string Name => Weapons.Gun;
        
        public Gun(float bulletSpeed, float reloadTime) {
            BulletSpeed = bulletSpeed;
            _reloadTime = reloadTime;
            CurState    = WeaponState.CHARGED;
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