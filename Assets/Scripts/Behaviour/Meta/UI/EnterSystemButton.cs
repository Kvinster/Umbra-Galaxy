using UnityEngine;
using UnityEngine.UI;

using STP.Common;
using STP.State.Meta;

namespace STP.Behaviour.Meta.UI {
    public sealed class EnterSystemButton : MonoBehaviour {
        public Button Button;

        MetaUiCanvas                 _owner;
        PlayerShipMovementController _playerShipMovementController;
        MetaTimeManager              _timeManager;
        StarSystemsManager           _starSystemsManager;

        void Reset() {
            Button = GetComponent<Button>();
        }

        void OnDestroy() {
            if ( _timeManager ) {
                _timeManager.OnPausedChanged -= OnPauseChanged;
            }
        }

        public void CommonInit(MetaUiCanvas owner, PlayerShipMovementController playerShipMovementController,
            MetaTimeManager timeManager, StarSystemsManager starSystemsManager) {
            _owner                        = owner;
            _playerShipMovementController = playerShipMovementController;
            _timeManager                  = timeManager;
            _starSystemsManager           = starSystemsManager;

            _playerShipMovementController.OnCurSystemChanged += OnPlayerShipCurSystemChanged;
            _timeManager.OnPausedChanged                     += OnPauseChanged;

            Button.onClick.AddListener(OnClick);
        }

        void OnPauseChanged(bool isPaused) {
            UpdateActive(isPaused, _playerShipMovementController.CurSystem.Id);
        }

        void OnPlayerShipCurSystemChanged(string playerCurSystem) {
            UpdateActive(_timeManager.IsPaused, playerCurSystem);
        }

        void OnClick() {
            _owner.ShowFactionSystemWindow(_playerShipMovementController.CurSystem.Id);
        }

        void UpdateActive(bool isPaused, string playerCurSystem) {
            Button.gameObject.SetActive(
                isPaused && (_starSystemsManager.GetStarSystem(playerCurSystem).Type == StarSystemType.Faction) &&
                StarSystemsController.Instance.GetFactionSystemActive(playerCurSystem));
        }
    }
}
