namespace STP.Gameplay.Weapon.Common {
    public interface IWeaponControl {
        void       UpdateControl(float timePassed);
        BaseWeapon GetControlledWeapon();
    }

    public abstract class BaseWeaponControl<T> : IWeaponControl where T : BaseWeapon {
        protected readonly T Weapon;

        protected BaseWeaponControl(T weapon) {
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