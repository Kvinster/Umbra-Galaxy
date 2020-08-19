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

        const string EscortTemplate =
            "Protect <color=orange>{0}'s</color> scientists expedition into <color=orange>{1}</color> by <color=orange>Day {2}</color>";

        const string ReclaimSystemTemplate =
            "Help reclaim system <color=orange>{0}</color> on <color=orange>Day {1}</color>";

        const string DefendSystemTemplate =
            "Help defend system <color=orange>{0}</color> on <color=orange>Day {1}</color>";

        const string DeliveryTemplate =
            "Deliver <color=orange>{0}</color> to system <color=orange>{1}</color> by <color=orange>Day {2}</color>";
        
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
                case QuestType.Escort: {
                    Text.text = string.Format(EscortTemplate,
                        _starSystemsController.GetStarSystemName(_curQuestState.OriginSystemId),
                        _starSystemsController.GetStarSystemName(_curQuestState.DestSystemId),
                        _curQuestState.ExpirationDay);
                    break;
                }
                case QuestType.ReclaimSystem: {
                    Text.text = string.Format(ReclaimSystemTemplate,
                        _starSystemsController.GetStarSystemName(_curQuestState.DestSystemId),
                        _curQuestState.ExpirationDay);
                    break;
                }
                case QuestType.DefendSystem: {
                    Text.text = string.Format(DefendSystemTemplate,
                        _starSystemsController.GetStarSystemName(_curQuestState.DestSystemId),
                        _curQuestState.ExpirationDay);
                    break;
                }
                case QuestType.Delivery when _curQuestState is DeliveryQuestState questState: {
                    Text.text = string.Format(DeliveryTemplate, questState.ResourceType,
                        _starSystemsController.GetStarSystemName(questState.DestSystemId), questState.ExpirationDay);
                    break;
                }
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
