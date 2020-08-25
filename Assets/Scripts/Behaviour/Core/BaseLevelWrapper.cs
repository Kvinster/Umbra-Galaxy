using STP.Behaviour.Starter;

namespace STP.Behaviour.Core {
    public abstract class BaseLevelWrapper : CoreComponent {
        public LevelQuestState LevelQuestState { get; protected set; }

        public override void Init(CoreStarter starter) { }
    }
}