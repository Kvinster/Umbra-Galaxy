using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.LaserWeapon {
    public class Laser : BaseWeapon {
        public override string Name => Weapons.Laser;
        
        public void TryShoot() {
            if ( CurState != WeaponState.FIRE ) {
                CurState = WeaponState.FIRE;
            }
        }
        
        public void TryStopShoot() {
            CurState = WeaponState.CHARGED;
        }

        protected override void AutoTransitions(float passedTime) { }
    }

}