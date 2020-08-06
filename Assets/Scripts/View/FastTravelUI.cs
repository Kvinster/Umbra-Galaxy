using UnityEngine;
using UnityEngine.UI;

using STP.Gameplay;
using STP.Utils;

using TMPro;

namespace STP.View {
    public class FastTravelUI : GameBehaviour{
        const string FastTravelText = "Fast travel";
        
        public Image    ButtonImage;
        public Button   FastTravelButton;
        public TMP_Text ButtonText;
        
        CoreManager      _coreManager;
        FastTravelEngine _fastTravelEngine;
        
        Color _defaultColor; 
        
        bool HasAnyAction => Mathf.Abs(Input.GetAxis("Horizontal")) > float.Epsilon ||
                             Mathf.Abs(Input.GetAxis("Vertical")) > float.Epsilon ||
                             Input.GetButton("Fire1");
        
        bool HasPressedHotKey => Input.GetButton("FastTravel");
        
        float LeftTime => _fastTravelEngine.Timer.LeftTime;
        
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, FastTravelButton);
        
        public void Init(CoreManager coreManager) {
            _coreManager      = coreManager;
            _fastTravelEngine = coreManager.FastTravelEngine;
            _defaultColor     = ButtonImage.color;
            FastTravelButton.onClick.AddListener(TryStartEngine);
        }

        void TryStartEngine() {
            if ( HasAnyAction ) {
                ButtonImage.color = Color.red;
                return;
            }
            _fastTravelEngine.TryStartEngine(_coreManager.GoToMeta);
        }
        
        void Update() {
            if ( HasPressedHotKey ) {
                TryStartEngine();
            }
            if ( HasAnyAction ) {
                _fastTravelEngine.StopEngine();
            }
            ButtonImage.color = (HasAnyAction && HasPressedHotKey) ? Color.red : _defaultColor;
            
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