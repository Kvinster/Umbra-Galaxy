using STP.Gameplay;
using STP.Utils;

namespace STP.View {
    public class CoreUI : CoreBehaviour {

        public ItemCounterUI     ItemCounterUI;
        public FastTravelUI      FastTravelUI;
        public MotherShipPointer Pointer;
        
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, FastTravelUI, ItemCounterUI, Pointer);

        public override void Init(CoreStarter starter) {
            ItemCounterUI.Init(starter);
            FastTravelUI.Init(starter);
            Pointer.Init(starter.CoreManager.PlayerShipState);
        }
    }
}
