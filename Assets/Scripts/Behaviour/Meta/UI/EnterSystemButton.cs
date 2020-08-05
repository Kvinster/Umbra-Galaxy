using UnityEngine;
using UnityEngine.UI;

using STP.Common;
using STP.State;
using STP.State.Meta;

namespace STP.Behaviour.Meta.UI {
    public sealed class EnterSystemButton : MonoBehaviour {
        public Button Button;

        StarSystemUiManager _owner;
        MetaTimeManager     _timeManager;
        StarSystemsManager  _starSystemsManager;
        PlayerState         _playerState;

        void Reset() {
            Button = GetComponent<Button>();
        }

        void OnDestroy() {
            _owner.OnStarSystemScreenActiveChanged -= OnStarSystemScreenActiveChanged;
            _timeManager.OnPausedChanged           -= OnPauseChanged;
            _playerState.OnCurSystemChanged        -= OnPlayerCurSystemChanged;
        }

        public void Init(StarSystemUiManager owner, MetaTimeManager timeManager,
            StarSystemsManager starSystemsManager) {
            _owner                        = owner;
            _timeManager                  = timeManager;
            _starSystemsManager           = starSystemsManager;

            _playerState = PlayerState.Instance;

            _owner.OnStarSystemScreenActiveChanged += OnStarSystemScreenActiveChanged;
            _timeManager.OnPausedChanged           += OnPauseChanged;
            _playerState.OnCurSystemChanged        += OnPlayerCurSystemChanged;
            UpdateActive(_timeManager.IsPaused, _playerState.CurSystemId, _owner.IsStarSystemScreenActive);

            Button.onClick.AddListener(OnClick);
        }

        void OnPauseChanged(bool isPaused) {
            UpdateActive(isPaused, _playerState.CurSystemId, _owner.IsStarSystemScreenActive);
        }

        void OnPlayerCurSystemChanged(string playerCurSystem) {
            UpdateActive(_timeManager.IsPaused, playerCurSystem, _owner.IsStarSystemScreenActive);
        }

        void OnStarSystemScreenActiveChanged(bool isStarSystemScreenActive) {
            UpdateActive(_timeManager.IsPaused, _playerState.CurSystemId, isStarSystemScreenActive);
        }

        void OnClick() {
            _owner.ShowStarSystemScreen();
        }

        void UpdateActive(bool isPaused, string playerCurSystem, bool isStarSystemScreenShown) {
            Button.gameObject.SetActive(
                isPaused && (_starSystemsManager.GetStarSystem(playerCurSystem).Type == StarSystemType.Faction) &&
                StarSystemsController.Instance.GetFactionSystemActive(playerCurSystem) && !isStarSystemScreenShown);
        }
    }
}
