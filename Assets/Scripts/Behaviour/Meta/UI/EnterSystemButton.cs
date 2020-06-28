using UnityEngine;
using UnityEngine.UI;

using STP.State;

namespace STP.Behaviour.Meta.UI {
    public sealed class EnterSystemButton : MonoBehaviour {
        public Button Button;

        MetaUiCanvas    _owner;
        PlayerShip      _playerShip;
        MetaTimeManager _timeManager;

        void Reset() {
            Button = GetComponent<Button>();
        }

        void OnDestroy() {
            if ( _timeManager ) {
                _timeManager.OnPausedChanged -= OnPauseChanged;
            }
        }

        public void CommonInit(MetaUiCanvas owner, PlayerShip playerShip, MetaTimeManager timeManager) {
            _owner       = owner;
            _playerShip  = playerShip;
            _timeManager = timeManager;

            _playerShip.OnCusSystemChanged += OnPlayerShipCurSystemChanged;
            _timeManager.OnPausedChanged   += OnPauseChanged;

            Button.onClick.AddListener(OnClick);
        }

        void OnPauseChanged(bool isPaused) {
            UpdateActive(isPaused, _playerShip.CurSystem.Name);
        }

        void OnPlayerShipCurSystemChanged(string playerCurSystem) {
            UpdateActive(_timeManager.IsPaused, playerCurSystem);
        }

        void OnClick() {
            _owner.ShowFactionSystemWindow(_playerShip.CurSystem.Name);
        }

        void UpdateActive(bool isPaused, string playerCurSystem) {
            Button.gameObject.SetActive(
                isPaused && StarSystemsController.Instance.GetStarSystemActive(playerCurSystem));
        }
    }
}
