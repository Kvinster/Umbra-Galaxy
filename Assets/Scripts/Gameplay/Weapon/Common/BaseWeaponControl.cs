namespace STP.Gameplay.Weapon.Common {
    public interface IWeaponControl {
        void       UpdateControl(float timePassed);
        BaseWeapon GetControlledWeapon();
    }
    
    public class BaseWeaponControl<T> : IWeaponControl where T : BaseWeapon {
        protected T Weapon;

        public BaseWeaponControl(T weapon) {
            Weapon = weapon;
        }

        public virtual void UpdateControl(float timePassed) {
            Weapon.UpdateState(timePassed);
        }

        public BaseWeapon GetControlledWeapon() {
            return Weapon;
        }
    }

}