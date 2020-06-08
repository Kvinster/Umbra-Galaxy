using System;
using UnityEngine;
using UnityEngine.UI;

namespace STP.Behaviour.Meta.UI {
    public sealed class EnterSystemButton : MonoBehaviour {
        public Image  Image;
        public Button Button;

        MetaUiCanvas    _owner;
        MetaTimeManager _timeManager;

        string _curStarSystemName;

        void Reset() {
            Image  = GetComponent<Image>();
            Button = GetComponent<Button>();
        }

        void OnDestroy() {
            if ( _timeManager ) {
                _timeManager.OnPausedChanged -= OnPauseChanged;
            }
        }

        public void CommonInit(BaseStarSystem startStarSystem, MetaUiCanvas owner, MetaTimeManager timeManager) {
            _curStarSystemName = startStarSystem.Name;
            _owner             = owner;
            _timeManager       = timeManager;
            
            _timeManager.OnPausedChanged += OnPauseChanged;

            Button.onClick.AddListener(OnClick);
        }

        public void Init(string starSystemName) {
            _curStarSystemName = starSystemName;
        }

        void OnPauseChanged(bool isPaused) {
            Button.gameObject.SetActive(isPaused);
        }

        void OnClick() {
            _owner.ShowFactionSystemWindow(_curStarSystemName);
        }
    }
}
