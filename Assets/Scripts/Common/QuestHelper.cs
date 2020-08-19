using UnityEngine;

using STP.State;
using STP.State.QuestStates;

namespace STP.Common {
    public sealed class QuestHelper {
        readonly QuestsController _questsController;
        readonly PlayerController _playerController;

        public QuestHelper(QuestsController questsController, PlayerController playerController) {
            _questsController = questsController;
            _playerController = playerController;
        }

        public bool TryCreateRandomQuest(string originSystemId, out BaseQuestState questState) {
            questState = _questsController.TryPrepareQuest(originSystemId);
            return (questState != null);
        }

        public bool TryStartQuest(BaseQuestState questState) {
            if ( _questsController.TryStartQuest(questState) ) {
                HandleQuestStart(questState);
                return true;
            }
            return false;
        }

        public bool TryFinishQuest(BaseQuestState questState) {
            if ( questState.Status != QuestStatus.Started ) {
                Debug.LogErrorFormat("Invalid quest status '{0}'", questState.Status.ToString());
                return false;
            }
            bool canFinish;
            switch ( questState.QuestType ) {
                case QuestType.GatherResource: {
                    Debug.LogError("GatherResources quests don't support status Finished");
                    canFinish = false;
                    break;
                }
                case QuestType.Escort: {
                    // TODO: check that Escort mission was finished
                    canFinish = true;
                    break;
                }
                case QuestType.ReclaimSystem: {
                    // TODO: check that ReclaimSystem mission was finished
                    canFinish = true;
                    break;
                }
                case QuestType.DefendSystem: {
                    // TODO: check that DefendSystem mission was finished
                    canFinish = true;
                    break;
                }
                case QuestType.Delivery: {
                    Debug.LogError("Delivery quests don't support status Finished");
                    canFinish = false;
                    break;
                }
                default: {
                    Debug.LogErrorFormat("Unsupported quest type '{0}'", questState.QuestType.ToString());
                    return false;
                }
            }
            return (canFinish && _questsController.TryFinishQuest(questState));
        }

        public bool TryCompleteQuest(BaseQuestState questState) {
            if ( _questsController.TryCompleteQuest(questState) ) {
                HandleQuestComplete(questState);
                return true;
            }
            return false;
        }

        public bool HasReadyToCompleteQuest() {
            return _questsController.HasReadyToCompleteQuest();
        }

        public BaseQuestState GetReadyToCompleteQuest() {
            return _questsController.GetReadyToCompleteQuest();
        }
        
        void HandleQuestStart(BaseQuestState baseQuestState) {
            switch ( baseQuestState.QuestType ) {
                case QuestType.Delivery when baseQuestState is DeliveryQuestState questState: {
                    if ( !_playerController.Inventory.TryAdd(questState.ResourceType, 1) ) {
                        Debug.LogErrorFormat("Can't add quest item '{0}' to player inventory", questState.ResourceType);
                    }
                    break;
                }
            }
        }
        
        void HandleQuestComplete(BaseQuestState baseQuestState) {
            // TODO: give full reward, preferably via some other manager
            _playerController.Money += baseQuestState.RewardInfo.Money;
            
            switch ( baseQuestState.QuestType ) {
                case QuestType.GatherResource when baseQuestState is GatherResourcesQuestState questState: {
                    if ( !_playerController.Inventory.TryRemove(questState.ResourceType, questState.ResourceAmount) ) {
                        Debug.LogErrorFormat("Can't remove resources for completing quest '{0}'", questState);
                    }
                    break;
                }
                case QuestType.Delivery when baseQuestState is DeliveryQuestState questState: {
                    if ( !_playerController.Inventory.TryRemove(questState.ResourceType, 1) ) {
                        Debug.LogErrorFormat("Can't remove quest item '{0}' from player inventory",
                            questState.ResourceType);
                    }
                    break;
                }
            }
        }
    }
}
