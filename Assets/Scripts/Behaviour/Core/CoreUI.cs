using STP.Behaviour.Starter;
using STP.Utils;

namespace STP.Behaviour.Core {
    public class CoreUI : CoreComponent {
        public FastTravelUI      FastTravelUI;

        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, FastTravelUI);

        public override void Init(CoreStarter starter) {
            FastTravelUI.Init(starter.CoreManager);
        }
    }
}
