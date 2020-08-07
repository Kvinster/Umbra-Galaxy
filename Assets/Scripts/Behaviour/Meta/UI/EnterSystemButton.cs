using UnityEngine;
using UnityEngine.UI;

using STP.Common;
using STP.State;
using STP.State.Meta;

namespace STP.Behaviour.Meta.UI {
    public sealed class EnterSystemButton : MonoBehaviour {
        public Button Button;

        StarSystemUiManager   _owner;
        MetaTimeManager       _timeManager;
        StarSystemsManager    _starSystemsManager;
        StarSystemsController _starSystemsController;
        PlayerController      _playerController;

        void Reset() {
            Button = GetComponent<Button>();
        }

        void OnDestroy() {
            _owner.OnStarSystemScreenActiveChanged -= OnStarSystemScreenActiveChanged;
            _timeManager.OnPausedChanged           -= OnPauseChanged;
            _playerController.OnCurSystemChanged   -= OnPlayerCurSystemChanged;
        }

        public void Init(StarSystemUiManager owner, MetaTimeManager timeManager,
            StarSystemsManager starSystemsManager, StarSystemsController starSystemsController,
            PlayerController playerController) {
            _owner                 = owner;
            _timeManager           = timeManager;
            _starSystemsManager    = starSystemsManager;
            _starSystemsController = starSystemsController;
            _playerController      = playerController;

            _owner.OnStarSystemScreenActiveChanged += OnStarSystemScreenActiveChanged;
            _timeManager.OnPausedChanged           += OnPauseChanged;
            _playerController.OnCurSystemChanged   += OnPlayerCurSystemChanged;
            UpdateActive(_timeManager.IsPaused, _playerController.CurSystemId, _owner.IsStarSystemScreenActive);

            Button.onClick.AddListener(OnClick);
        }

        void OnPauseChanged(bool isPaused) {
            UpdateActive(isPaused, _playerController.CurSystemId, _owner.IsStarSystemScreenActive);
        }

        void OnPlayerCurSystemChanged(string playerCurSystem) {
            UpdateActive(_timeManager.IsPaused, playerCurSystem, _owner.IsStarSystemScreenActive);
        }

        void OnStarSystemScreenActiveChanged(bool isStarSystemScreenActive) {
            UpdateActive(_timeManager.IsPaused, _playerController.CurSystemId, isStarSystemScreenActive);
        }

        void OnClick() {
            _owner.ShowStarSystemScreen();
        }

        void UpdateActive(bool isPaused, string playerCurSystem, bool isStarSystemScreenShown) {
            Button.gameObject.SetActive(
                isPaused && (_starSystemsManager.GetStarSystem(playerCurSystem).Type == StarSystemType.Faction) &&
                _starSystemsController.GetFactionSystemActive(playerCurSystem) && !isStarSystemScreenShown);
        }
    }
}
