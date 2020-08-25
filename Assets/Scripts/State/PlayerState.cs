using STP.Gameplay.Weapon.Common;

namespace STP.State {
    public sealed class PlayerState {
        public const int MaxFuel = 100;
        public const int StartHp = 100;
        
        public readonly PlayerInventory Inventory = new PlayerInventory();
        
        public string     CurSystemId;
        public int        Fuel;
        public int        Money;
        
        public PlayerShipState CurShipState = new PlayerShipState(StartHp); 
        
        public WeaponType CurWeaponType;
    }
}
