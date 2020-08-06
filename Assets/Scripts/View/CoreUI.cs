using STP.Gameplay;
using STP.Utils;

namespace STP.View {
    public class CoreUI : CoreBehaviour {
        public FastTravelUI      FastTravelUI;
        
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, FastTravelUI);

        public override void Init(CoreStarter starter) {
            FastTravelUI.Init(starter.CoreManager);
        }
    }
}
