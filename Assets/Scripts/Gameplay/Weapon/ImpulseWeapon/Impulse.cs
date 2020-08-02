using STP.Gameplay.Weapon.Chargeable;

using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.ImpulseWeapon {
    public class Impulse : ChargeableWeapon  {
        public override WeaponType Name         => WeaponType.Impulse;
        public override float      Damage       => 5f;
        protected override float   ChargingTime => 0.5f;
        
        public float ChargeProgress {
            get {
                switch ( CurState ) {
                    case WeaponState.Charged:
                        return 1f;
                    case WeaponState.Charge:
                        return Timer.NormalizedProgress;
                    default:
                        return 0f;
                }
            }
        }
    }
}