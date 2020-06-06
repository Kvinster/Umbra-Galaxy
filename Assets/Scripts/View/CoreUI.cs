using STP.Gameplay;
using UnityEngine.UI;

using STP.State;
using STP.Utils;

using TMPro;

namespace STP.View {
    public class CoreUI : CoreBehaviour {
        
        public ItemCounterUI ItemCounterUI;
        public Button        FastTravelButton;
    
        
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, FastTravelButton, ItemCounterUI);

        public override void Init(CoreStarter starter) {
        }

    }
}
