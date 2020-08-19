using STP.Common;

namespace STP.State.QuestStates {
    public sealed class GatherResourcesQuestState : BaseQuestState {
        public readonly string ResourceType;
        public readonly int    ResourceAmount;

        public GatherResourcesQuestState(string resourceType, int resourceAmount, int expirationDay,
            string originSystemId, string destSystemId, string rewardSystemId, RewardInfo rewardInfo,
            QuestStatus status = QuestStatus.Created) : base(QuestType.GatherResource, expirationDay, originSystemId,
            destSystemId, rewardSystemId, rewardInfo, status) {
            ResourceType   = resourceType;
            ResourceAmount = resourceAmount;
        }
    }
}
