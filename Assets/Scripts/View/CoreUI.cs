using STP.Utils;
using TMPro;
using UnityEngine.UI;

namespace STP.View {
    public class CoreUI : CoreComponent {
        public Button    FastTravelButton;
        public TMP_Text  InventoryCountText;
    
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, FastTravelButton, InventoryCountText);

        public override void Init(CoreStarter starter) {
            
        }
    }
}
