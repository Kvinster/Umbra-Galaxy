using UnityEngine.UI;

using STP.Gameplay;
using STP.Utils;

namespace STP.View {
    public class CoreUI : CoreBehaviour {
        
        public ItemCounterUI ItemCounterUI;
        public Button        FastTravelButton;
    
        
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, FastTravelButton, ItemCounterUI);

        public override void Init(CoreStarter starter) {
            FastTravelButton.onClick.AddListener(starter.CoreManager.TeleportToMothership);
        }

    }
}
