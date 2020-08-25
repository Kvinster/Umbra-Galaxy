using STP.Behaviour.Starter;
using STP.Utils;

namespace STP.Behaviour.Core {
    public abstract class BaseLevelWrapper : GameComponent {
        public LevelQuestState LevelQuestState { get; protected set; }

        public virtual void Init(CoreStarter starter) { }
    }
}