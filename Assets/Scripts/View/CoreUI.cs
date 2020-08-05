using STP.Gameplay;
using STP.Utils;

namespace STP.View {
    public class CoreUI : CoreBehaviour {
        public ItemCounterUI     ItemCounterUI;
        public FastTravelUI      FastTravelUI;
        
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, FastTravelUI, ItemCounterUI);

        public override void Init(CoreStarter starter) {
            ItemCounterUI.Init(starter.CoreManager);
            FastTravelUI.Init(starter.CoreManager);
        }
    }
}
