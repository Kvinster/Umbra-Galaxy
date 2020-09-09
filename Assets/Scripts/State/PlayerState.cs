using STP.Gameplay.Weapon.Common;

namespace STP.State {
    public sealed class PlayerState {
        public const int MaxFuel   = 100;
        public const int MaxShipHp = 100;

        public readonly PlayerInventory Inventory    = new PlayerInventory();
        public readonly PlayerShipState CurShipState = new PlayerShipState(MaxShipHp);

        public string CurSystemId;
        public int    Fuel;
        public int    Money;

        public WeaponType CurWeaponType;
    }
}
