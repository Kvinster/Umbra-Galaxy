using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.Common;
using STP.State;

namespace STP.Behaviour.Meta.UI {
    public sealed class PlayerUi : BaseMetaComponent {
        public PlayerInventoryButton InventoryButton;
        public Button                EnterSystemButton;

        StarSystemsManager _starSystemsManager;
        MetaTimeManager    _timeManager;
        PlayerController   _playerController;

        string PlayerCurSystemId => _playerController.CurSystemId;
        bool   IsPaused          => _timeManager.IsPaused;

        void OnDestroy() {
            _timeManager.OnPausedChanged            -= OnTimePausedChanged;
            _playerController.OnCurSystemChanged -= OnPlayerCurSystemChanged;
        }

        protected override void InitInternal(MetaStarter starter) {
            _starSystemsManager = starter.StarSystemsManager;
            _timeManager        = starter.TimeManager;
            _playerController   = starter.PlayerController;
            
            _timeManager.OnPausedChanged         += OnTimePausedChanged;
            _playerController.OnCurSystemChanged += OnPlayerCurSystemChanged;

            UpdateEnterSystemButtonActive(IsPaused,
                _starSystemsManager.GetStarSystem(PlayerCurSystemId).Type == StarSystemType.Faction);
        }

        void OnPlayerCurSystemChanged(string curSystemId) {
            UpdateEnterSystemButtonActive(IsPaused,
                _starSystemsManager.GetStarSystem(curSystemId).Type == StarSystemType.Faction);
        }
        
        void OnTimePausedChanged(bool isPaused) {
            UpdateEnterSystemButtonActive(isPaused,
                _starSystemsManager.GetStarSystem(PlayerCurSystemId).Type == StarSystemType.Faction);
        }

        void UpdateEnterSystemButtonActive(bool isPaused, bool isCurSystemFaction) {
            EnterSystemButton.gameObject.SetActive(isPaused && isCurSystemFaction);
        }
    }
}
