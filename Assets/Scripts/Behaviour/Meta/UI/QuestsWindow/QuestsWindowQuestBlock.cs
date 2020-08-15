using UnityEngine;

using STP.Common;
using STP.State.Meta;
using STP.State.QuestStates;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.Meta.UI.QuestsWindow {
    public sealed class QuestsWindowQuestBlock : GameBehaviour {
        const string GatherResourcesTemplate =
            "Deliver <color=orange>{0}</color> of <color=orange>{1}</color> to <color=orange>{2}</color> by <color=orange>Day {3}</color>";
        
        [NotNull] public TMP_Text Text;

        BaseQuestState        _curQuestState;
        StarSystemsController _starSystemsController;

        public bool IsActive => (_curQuestState != null);

        public void Init(BaseQuestState questState, StarSystemsController starSystemsController) {
            _curQuestState         = questState;
            _starSystemsController = starSystemsController;

            InitText();
        }

        void InitText() {
            switch ( _curQuestState.QuestType ) {
                case QuestType.GatherResource when _curQuestState is GatherResourcesQuestState questState: {
                    Text.text = string.Format(GatherResourcesTemplate, questState.ResourceAmount,
                        questState.ResourceType, _starSystemsController.GetStarSystemName(questState.RewardSystemId),
                        questState.ExpirationDay);
                    break;
                }
                case QuestType.Escort:
                    break;
                case QuestType.ReclaimSystem:
                    break;
                case QuestType.DefendSystem:
                    break;
                case QuestType.Delivery:
                    break;
                case QuestType.Unknown:
                    break;
                default: {
                    Debug.LogErrorFormat("Unsupported quest type '{0}'", _curQuestState.QuestType.ToString());
                    break;
                }
            }
        }

        public void Deinit() {
            _curQuestState         = null;
            _starSystemsController = null;
        }
    }
}
