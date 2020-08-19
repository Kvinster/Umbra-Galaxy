using STP.Common;

namespace STP.State.QuestStates {
    public sealed class ReclaimSystemQuestState : BaseQuestState {
        public ReclaimSystemQuestState(int expirationDay, string originSystemId, string destSystemId,
            string rewardSystemId, RewardInfo rewardInfo, QuestStatus status = QuestStatus.Created) : base(
            QuestType.ReclaimSystem, expirationDay, originSystemId, destSystemId, rewardSystemId, rewardInfo,
            status) { }
    }
}
