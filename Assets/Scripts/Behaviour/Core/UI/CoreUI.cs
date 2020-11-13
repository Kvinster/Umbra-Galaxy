using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.UI {
    public class CoreUI : BaseCoreComponent {
        [NotNull] public FastTravelUI       FastTravelUI;
        [NotNull] public LevelQuestStatusUI LevelQuestUI;

        protected override void InitInternal(CoreStarter starter) {
            FastTravelUI.Init(starter.CoreManager);
            LevelQuestUI.Init(starter.LevelWrapper);
        }
    }
}
