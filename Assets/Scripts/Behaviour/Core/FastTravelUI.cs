using UnityEngine;
using UnityEngine.UI;

using STP.Events;
using STP.Gameplay;
using STP.Utils;
using STP.Utils.Events;
using STP.Utils.GameComponentAttributes;
using System;
using TMPro;

namespace STP.Behaviour.Core {
    public class FastTravelUI : GameComponent {
        const string FastTravelText = "Fast travel";
        
        [NotNull] public Image    ButtonImage;
        [NotNull] public Button   FastTravelButton;
        [NotNull] public TMP_Text ButtonText;
        
        CoreManager      _coreManager;
        FastTravelEngine _fastTravelEngine;
        
        Color _defaultColor; 
        
        bool HasAnyAction => Mathf.Abs(Input.GetAxis("Horizontal")) > float.Epsilon ||
                             Mathf.Abs(Input.GetAxis("Vertical")) > float.Epsilon ||
                             Input.GetButton("Fire1");
        
        bool HasPressedHotKey => Input.GetButton("FastTravel");
        
        float LeftTime => _fastTravelEngine.Timer.TimeLeft;
        
        public void Init(CoreManager coreManager) {
            _coreManager      = coreManager;
            _fastTravelEngine = coreManager.FastTravelEngine;
            _defaultColor     = ButtonImage.color;
            FastTravelButton.onClick.AddListener(TryStartEngine);
            gameObject.SetActive(false);
            EventManager.Subscribe<QuestCompleted>(OnQuestComplete);
        }

        void OnDestroy() {
            EventManager.Unsubscribe<QuestCompleted>(OnQuestComplete);
        }

        void OnQuestComplete(QuestCompleted e) {
            gameObject.SetActive(true);
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
                    ButtonText.text = HRTime.ConvertToSMString(LeftTime);
                    break;
                default:
                    Debug.LogWarning($"Unknown state {_fastTravelEngine.State}. Ignored.");
                    break;
            }
        }
    }
}