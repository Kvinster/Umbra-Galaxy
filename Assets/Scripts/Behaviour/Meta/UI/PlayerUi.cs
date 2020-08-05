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

        string PlayerCurSystemId => PlayerState.Instance.CurSystemId;
        bool   IsPaused          => _timeManager.IsPaused;

        void OnDestroy() {
            _timeManager.OnPausedChanged            -= OnTimePausedChanged;
            PlayerState.Instance.OnCurSystemChanged -= OnPlayerCurSystemChanged;
        }

        protected override void InitInternal(MetaStarter starter) {
            _starSystemsManager = starter.StarSystemsManager;
            _timeManager        = starter.TimeManager;
            
            _timeManager.OnPausedChanged            += OnTimePausedChanged;
            PlayerState.Instance.OnCurSystemChanged += OnPlayerCurSystemChanged;

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
