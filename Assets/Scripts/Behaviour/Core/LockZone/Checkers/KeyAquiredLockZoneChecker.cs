using STP.Behaviour.Starter;
using STP.State;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.LockZone.Checkers {
    public class KeyAquiredLockZoneChecker : BaseLockZoneChecker {
        [NotNullOrEmpty] public string KeyName;
        
        CorePlayerController _playerController;

        public override bool LockConditionActive => _playerController?.HasKey(KeyName) ?? false;

        public override void Init(CoreStarter starter) {
            _playerController = starter.CorePlayerController;
        }
    }
}