namespace STP.Gameplay.Weapon.Common {
    public enum WeaponState {
        Idle,
        Charge,
        Charged,
        Fire
    }

    public enum WeaponType {
        Gun             = 0,
        Shotgun         = 1,
        Laser           = 2,
        Lance           = 3,
        Impulse         = 4,
        MissileLauncher = 5,
        
        Unknown = -1,
    }

    public abstract class BaseWeapon {
        public virtual WeaponType Name => WeaponType.Unknown;

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
