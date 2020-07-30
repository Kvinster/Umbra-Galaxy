using UnityEngine;
using UnityEngine.UI;

using STP.Gameplay;
using STP.Utils;

using TMPro;

namespace STP.View {
    public class FastTravelUI : GameBehaviour{
        const string FastTravelText = "Fast travel";
        
        public Image    WarningImage;
        public Button   FastTravelButton;
        public TMP_Text ButtonText;
        
        CoreManager      _coreManager;
        FastTravelEngine _fastTravelEngine;
        
        bool HasAnyAction => Mathf.Abs(Input.GetAxis("Horizontal")) > float.Epsilon ||
                             Mathf.Abs(Input.GetAxis("Vertical")) > float.Epsilon ||
                             Input.GetButton("Fire1");
        
        bool HasPressedHotKey => Input.GetButton("FastTravel");
        
        float LeftTime => _fastTravelEngine.Timer.LeftTime;
        
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, FastTravelButton);
        
        public void Init(CoreManager coreManager) {
            _coreManager      = coreManager;
            _fastTravelEngine = coreManager.FastTravelEngine;
            FastTravelButton.onClick.AddListener(TryStartEngine);
        }

        void TryStartEngine() {
            if ( HasAnyAction ) {
                WarningImage.enabled = true;
                return;
            }
            _fastTravelEngine.TryStartEngine(_coreManager.TeleportToMothership);
        }
        
        void Update() {
            if ( HasPressedHotKey ) {
                TryStartEngine();
            }
            if ( HasAnyAction ) {
                _fastTravelEngine.StopEngine();
            }
            WarningImage.enabled = HasAnyAction && HasPressedHotKey;
            
            switch ( _fastTravelEngine.State ) {
                case EngineState.IDLE:
                case EngineState.CHARGED:
                    ButtonText.text = FastTravelText;
                    break;
                case EngineState.CHARGING:
                    ButtonText.text = new HRTime().SetSeconds(LeftTime).GetSM();
                    break;
                default:
                    Debug.LogWarning($"Unknown state {_fastTravelEngine.State}. Ignored.");
                    break;
            }
        }
    }
}