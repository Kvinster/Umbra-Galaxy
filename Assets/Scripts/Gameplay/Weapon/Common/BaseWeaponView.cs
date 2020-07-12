using STP.Utils;

namespace STP.Gameplay.Weapon.Common {
    public abstract class BaseWeaponView : GameBehaviour{
        protected override void CheckDescription() { }
        
        public abstract void Init(CoreStarter starter, BaseShip ownerShip, BaseWeapon ownerWeapon); 
    }
}