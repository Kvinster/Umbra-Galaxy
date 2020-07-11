using UnityEngine;
using UnityEngine.UI;

using STP.Behaviour.Common;
using STP.State;

using TMPro;

namespace STP.Behaviour.Meta.UI.FactionSystemWindow {
    public sealed class PlayerInventoryItemView : MonoBehaviour {
        public Image    ItemIcon;
        public TMP_Text ItemNameText;
        public TMP_Text ItemAmountText;
        public Button   Button;

        string _itemName;
        string _starSystemId;
        
        InventoryItemInfos _inventoryItemInfos;

        MetaUiCanvas _metaUiCanvas;

        public void CommonInit(InventoryItemInfos inventoryItemInfos, MetaUiCanvas metaUiCanvas) {
            _metaUiCanvas       = metaUiCanvas;
            _inventoryItemInfos = inventoryItemInfos;
            Button.onClick.AddListener(OnClick);
        }

        public void Init(string itemName, string starSystemId) {
            _itemName     = itemName;
            _starSystemId = starSystemId;
            
            if ( !PlayerState.Instance.HasInInventory(itemName) ) {
                Debug.LogErrorFormat("No item '{0}' in player's inventory", itemName);
                return;
            }
            ItemNameText.text   = itemName;
            ItemAmountText.text = PlayerState.Instance.GetInventoryItemAmount(itemName).ToString();
            ItemIcon.sprite     = _inventoryItemInfos.GetItemInventoryIcon(itemName);
        }

        void OnClick() {
            _metaUiCanvas.ShowInventoryItemSellWindow(_itemName, _starSystemId);
        }
    }
}
