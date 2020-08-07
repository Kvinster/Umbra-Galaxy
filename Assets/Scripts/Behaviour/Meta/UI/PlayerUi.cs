using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.Common;
using STP.State;
using STP.State.Meta;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Meta.UI {
    public sealed class PlayerUi : BaseMetaComponent {
        // TODO: what's with this unused button?
        [NotNull] public PlayerInventoryButton InventoryButton;
        [NotNull] public Button                EnterSystemButton;
        
        MetaTimeManager       _timeManager;
        StarSystemsController _starSystemsController;
        PlayerController      _playerController;

        string PlayerCurSystemId => _playerController.CurSystemId;
        bool   IsPaused          => _timeManager.IsPaused;

        void OnDestroy() {
            _timeManager.OnPausedChanged         -= OnTimePausedChanged;
            _playerController.OnCurSystemChanged -= OnPlayerCurSystemChanged;
        }

        protected override void InitInternal(MetaStarter starter) {
            _timeManager           = starter.TimeManager;
            _starSystemsController = starter.StarSystemsController;
            _playerController      = starter.PlayerController;
            
            _timeManager.OnPausedChanged         += OnTimePausedChanged;
            _playerController.OnCurSystemChanged += OnPlayerCurSystemChanged;

            UpdateEnterSystemButtonActive(IsPaused,
                _starSystemsController.GetStarSystemType(PlayerCurSystemId) == StarSystemType.Faction);
        }

        void OnPlayerCurSystemChanged(string curSystemId) {
            UpdateEnterSystemButtonActive(IsPaused,
                _starSystemsController.GetStarSystemType(curSystemId) == StarSystemType.Faction);
        }
        
        void OnTimePausedChanged(bool isPaused) {
            UpdateEnterSystemButtonActive(isPaused,
                _starSystemsController.GetStarSystemType(PlayerCurSystemId) == StarSystemType.Faction);
        }

        void UpdateEnterSystemButtonActive(bool isPaused, bool isCurSystemFaction) {
            EnterSystemButton.gameObject.SetActive(isPaused && isCurSystemFaction);
        }
    }
}
