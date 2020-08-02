using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.LaserWeapon {
    public class Laser : BaseWeapon {
        public override WeaponType Name => WeaponType.Laser;
        
        public void TryShoot() {
            if ( CurState != WeaponState.Fire ) {
                CurState = WeaponState.Fire;
            }
        }
        
        public void TryStopShoot() {
            CurState = WeaponState.Charged;
        }

        protected override void AutoTransitions(float passedTime) { }
    }

}