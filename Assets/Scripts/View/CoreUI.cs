using UnityEngine;
using UnityEngine.UI;

using STP.Gameplay;
using STP.Utils;

namespace STP.View {
    public class CoreUI : CoreBehaviour {
        const float FastTravelTimer = 3f;
        
        public ItemCounterUI ItemCounterUI;
        public Button        FastTravelButton;

        float      _fastTravelActivationTimer;
        GameObject _fastTravelButtonGO;

        bool HasAnyAction => Mathf.Abs(Input.GetAxis("Horizontal")) > float.Epsilon ||
                             Mathf.Abs(Input.GetAxis("Vertical")) > float.Epsilon ||
                             Input.GetButton("Fire1");
        
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, FastTravelButton, ItemCounterUI);

        public override void Init(CoreStarter starter) {
            FastTravelButton.onClick.AddListener(starter.CoreManager.TeleportToMothership);
            _fastTravelActivationTimer = 0f;
            _fastTravelButtonGO        = FastTravelButton.gameObject;
            _fastTravelButtonGO.SetActive(false);
        }
        
        void Update() {
            _fastTravelActivationTimer += Time.deltaTime;
            if ( HasAnyAction ) {
                _fastTravelButtonGO.SetActive(false);
                _fastTravelActivationTimer = 0f;
            }

            if ( (_fastTravelActivationTimer > FastTravelTimer) && (!_fastTravelButtonGO.activeSelf) ) {
                _fastTravelButtonGO.SetActive(true);
            }
            
        }
    }
}
