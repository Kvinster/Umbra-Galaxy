using System;

using STP.Common;

namespace STP.State.QuestStates {
    public abstract class BaseQuestState {
        public readonly QuestType  QuestType;
        public readonly int        ExpirationDay;
        public readonly string     OriginSystemId;
        public readonly string     DestSystemId;
        public readonly string     RewardSystemId;
        public readonly RewardInfo RewardInfo;

        public QuestStatus Status;

        // readonly for now, later will be loaded
        public readonly string Id;

        protected BaseQuestState(QuestType questType, int expirationDay, string originSystemId, string destSystemId,
            string rewardSystemId, RewardInfo rewardInfo, QuestStatus status = QuestStatus.Created) {
            QuestType      = questType;
            ExpirationDay  = expirationDay;
            OriginSystemId = originSystemId;
            DestSystemId   = destSystemId;
            RewardSystemId = rewardSystemId;
            RewardInfo     = rewardInfo;

            Status = status;

            Id = Guid.NewGuid().ToString();
        }

        public override string ToString() {
            return
                $"(type: {QuestType.ToString()}, Status: {Status.ToString()}, ExpirationDay: {ExpirationDay}, " +
                $"OriginSystemId: {OriginSystemId}, DestSystemId: {DestSystemId}, RewardSystemId: {RewardSystemId}, " +
                $"Id: {Id})";
        }
    }
}
