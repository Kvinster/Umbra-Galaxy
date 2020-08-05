using STP.Gameplay;
using STP.State;
using STP.Utils;
using TMPro;

namespace STP.View {
    public class ItemCounterUI : GameBehaviour {
        const string TextFormat = "{0}/{1}";
        
        public TMP_Text  InventoryCountText;
        
        CoreManager  _coreManager;
        
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, InventoryCountText);

        public void Init(CoreManager manager) {
            _coreManager = manager;
        }
        
        void Update() {
            InventoryCountText.text = string.Format(TextFormat, _coreManager.UsedCapacity, PlayerInventory.Capacity);
        }
    }
}