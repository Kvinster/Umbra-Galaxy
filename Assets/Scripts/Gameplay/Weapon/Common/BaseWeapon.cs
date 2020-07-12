namespace STP.Gameplay.Weapon.Common {
    public enum WeaponState {
        IDLE,
        CHARGE,
        CHARGED,
        FIRE
    }

    public class Weapons {
        public const string Bullets = "bullets";
        public const string Laser   = "laser";
    }

    public abstract class BaseWeapon {
        public abstract string Name { get; }
        
        WeaponState _state;
        public WeaponState CurState {
            get => _state;
            protected set {
                _state = value;
                StateChanged?.Invoke(_state);
            }
        }

        public delegate void OnChangeState(WeaponState newWeaponState);

        public event OnChangeState StateChanged;

        public void UpdateState(float passedTime) {
            AutoTransitions(passedTime);
        }

        protected abstract void AutoTransitions(float passedTime);
    }

}