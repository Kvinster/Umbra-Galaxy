using STP.Gameplay.Weapon.Common;
using STP.Utils;

namespace STP.Gameplay.Weapon.GunWeapon {
    public class Gun : BaseWeapon {
        readonly Timer _timer = new Timer();

        float _reloadTime;

        public float BulletSpeed {get;}

        public override WeaponType Name => WeaponType.Gun;

        public Gun(float bulletSpeed, float reloadTime) {
            BulletSpeed = bulletSpeed;
            _reloadTime = reloadTime;
            CurState    = WeaponState.Charged;
        }

        public void TryShoot() {
            if ( CurState == WeaponState.Charged ) {
                CurState = WeaponState.Fire;
            }
        }

        protected override void Update(float passedTime) {
            switch ( CurState ) {
                case WeaponState.Idle:
                    _timer.Start(_reloadTime);
                    CurState = WeaponState.Charge;
                    break;
                case WeaponState.Charge:
                    if ( _timer.Tick(passedTime) ) {
                        _timer.Stop();
                        CurState = WeaponState.Charged;
                    }
                    break;
                case WeaponState.Fire:
                    CurState = WeaponState.Idle;
                    break;
            }
        }
    }
}