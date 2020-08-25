using STP.Behaviour.Starter;
using STP.Utils;

namespace STP.Gameplay.Weapon.Common {
    public abstract class BaseWeaponView : GameComponent{
        protected override void CheckDescription() { }
        
        public abstract void Init(CoreStarter starter, BaseShip ownerShip, BaseWeapon ownerWeapon); 
    }
}