using UnityEngine;
using UnityEngine.UI;

using STP.Events;
using STP.Utils;
using STP.Utils.Events;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.Core.UI {
    public sealed class LevelQuestStatusUI : GameComponent {
        const string TextFormat = "Quest status: {0}";
        
        [NotNull] public TMP_Text StatusText;
        [NotNull] public Image    Background;
        
        Color _defaultColor; 
        
        BaseLevelWrapper _levelWrapper;
        
        public void Init(BaseLevelWrapper levelWrapper) {
            _levelWrapper = levelWrapper;
            _defaultColor = Background.color;
            EventManager.Subscribe<QuestCompleted>(OnQuestComplete);
            EventManager.Subscribe<QuestFailed>(OnQuestFailed);
        }

        void OnDestroy() {
            EventManager.Unsubscribe<QuestCompleted>(OnQuestComplete);
            EventManager.Unsubscribe<QuestFailed>(OnQuestFailed);
        }

        void OnQuestComplete(QuestCompleted e) {
            StatusText.text = string.Format(TextFormat, _levelWrapper.LevelQuestState);
            SetBackgroundColor(LevelQuestState.Completed);
        }
        
        void OnQuestFailed(QuestFailed e) {
            StatusText.text = string.Format(TextFormat, _levelWrapper.LevelQuestState);
            SetBackgroundColor(LevelQuestState.Failed);
        }

        void SetBackgroundColor(LevelQuestState state) {
            switch ( state ) {
                case LevelQuestState.Failed: {
                    Background.color = Color.red;
                    break;
                }
                case LevelQuestState.Completed: {
                    Background.color = Color.green;
                    break;
                }
                default: {
                    Background.color = _defaultColor;
                    break;
                }
            }
        }
    }
}