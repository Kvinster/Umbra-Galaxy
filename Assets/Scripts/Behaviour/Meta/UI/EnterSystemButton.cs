using UnityEngine;
using UnityEngine.UI;

using STP.Common;
using STP.State;

namespace STP.Behaviour.Meta.UI {
    public sealed class EnterSystemButton : MonoBehaviour {
        public Button Button;

        MetaUiCanvas       _owner;
        PlayerShip         _playerShip;
        MetaTimeManager    _timeManager;
        StarSystemsManager _starSystemsManager;

        void Reset() {
            Button = GetComponent<Button>();
        }

        void OnDestroy() {
            if ( _timeManager ) {
                _timeManager.OnPausedChanged -= OnPauseChanged;
            }
        }

        public void CommonInit(MetaUiCanvas owner, PlayerShip playerShip, MetaTimeManager timeManager,
            StarSystemsManager starSystemsManager) {
            _owner              = owner;
            _playerShip         = playerShip;
            _timeManager        = timeManager;
            _starSystemsManager = starSystemsManager;

            _playerShip.OnCusSystemChanged += OnPlayerShipCurSystemChanged;
            _timeManager.OnPausedChanged   += OnPauseChanged;

            Button.onClick.AddListener(OnClick);
        }

        void OnPauseChanged(bool isPaused) {
            UpdateActive(isPaused, _playerShip.CurSystem.Id);
        }

        void OnPlayerShipCurSystemChanged(string playerCurSystem) {
            UpdateActive(_timeManager.IsPaused, playerCurSystem);
        }

        void OnClick() {
            _owner.ShowFactionSystemWindow(_playerShip.CurSystem.Id);
        }

        void UpdateActive(bool isPaused, string playerCurSystem) {
            Button.gameObject.SetActive(
                isPaused && (_starSystemsManager.GetStarSystem(playerCurSystem).Type == StarSystemType.Faction) &&
                StarSystemsController.Instance.GetFactionSystemActive(playerCurSystem));
        }
    }
}
