using UnityEngine.UI;

using STP.State;
using STP.Utils;

using TMPro;

namespace STP.View {
    public class CoreUI : CoreBehaviour {
        const string TextFormat = "{0}/{1}";
        
        public Button    FastTravelButton;
        public TMP_Text  InventoryCountText;
        
        CoreShipState _shipState;
    
        bool _isInited;
        
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, FastTravelButton, InventoryCountText);

        public override void Init(CoreStarter starter) {
            _shipState = starter.ShipState;
            _isInited  = true;
        }

        void Update() {
            if ( !_isInited ) {
                return;
            }
            InventoryCountText.text = string.Format(TextFormat, GetShipTotalItems(), _shipState.Capacity);
        }

        int GetShipTotalItems() {
            var count = 0;
            foreach (var item in _shipState.ShipInventory) {
                count += item.Value;
            }
            return count;
        }
    }
}
