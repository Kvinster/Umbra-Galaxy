using STP.Common;

namespace STP.State.QuestStates {
    public sealed class DefendSystemQuestState : BaseQuestState {
        public DefendSystemQuestState(int expirationDay, string destSystemId, string rewardSystemId,
            RewardInfo rewardInfo, QuestStatus status = QuestStatus.Created) : base(QuestType.DefendSystem,
            expirationDay, destSystemId, rewardSystemId, rewardInfo, status) { }
    }
}
