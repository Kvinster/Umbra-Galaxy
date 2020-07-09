namespace STP.Gameplay.WeaponGroup.Weapons {
    public enum WeaponState {
        IDLE,
        CHARGE,
        CHARGED,
        FIRE,
        COOLDOWN
    }

    public class Weapons {
        public const string Bullets = "bullets";
        public const string Laser   = "laser";
        public const string Lance   = "lance";
    }

    public abstract class BaseWeapon {
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