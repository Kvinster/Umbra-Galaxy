using STP.Common;

namespace STP.State.QuestStates {
    public sealed class DeliveryQuestState : BaseQuestState {
        public readonly string ResourceType;

        public DeliveryQuestState(string resourceType, int expirationDay, string destSystemId, string rewardSystemId,
            RewardInfo rewardInfo, QuestStatus status = QuestStatus.Created) : base(QuestType.Delivery,
            expirationDay, destSystemId, rewardSystemId, rewardInfo, status) {
            ResourceType = resourceType;
        }
    }
}
