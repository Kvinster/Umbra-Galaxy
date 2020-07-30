using STP.Gameplay.Weapon.Chargeable;

using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.ImpulseWeapon {
    public class Impulse : ChargeableWeapon  {
        public override string   Name         => Weapons.Impulse;
        public override float    Damage       => 5f;
        protected override float ChargingTime => 0.5f;
        
        public float ChargeProgress {
            get {
                switch ( CurState ) {
                    case WeaponState.CHARGED:
                        return 1f;
                    case WeaponState.CHARGE:
                        return Timer.NormalizedProgress;
                    default:
                        return 0f;
                }
            }
        }
    }
}