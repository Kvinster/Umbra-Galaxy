using System.Collections.Generic;

using STP.State.QuestStates;

namespace STP.State {
    public sealed class QuestsControllerState {
        public readonly List<BaseQuestState> QuestStates = new List<BaseQuestState>();
    }
}
