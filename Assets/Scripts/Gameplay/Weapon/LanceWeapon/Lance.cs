using STP.Gameplay.Weapon.Chargeable;

using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.LanceWeapon {
    public class Lance : ChargeableWeapon  {
        public override string   Name         => Weapons.Lance;
        public override float    Damage       => 5f;
        protected override float ChargingTime => 1f;
    }
}