using STP.Common;

namespace STP.State.QuestStates {
    public sealed class EscortQuestState : BaseQuestState {
        public EscortQuestState(int expirationDay, string originSystemId, string destSystemId, string rewardSystemId,
            RewardInfo rewardInfo, QuestStatus status = QuestStatus.Created) : base(QuestType.Escort, expirationDay,
            originSystemId, destSystemId, rewardSystemId, rewardInfo, status) { }
    }
}
