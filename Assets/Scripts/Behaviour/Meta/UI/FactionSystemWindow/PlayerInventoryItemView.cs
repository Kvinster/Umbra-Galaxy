using UnityEngine;
using UnityEngine.UI;

using System;

using STP.Behaviour.Common;
using STP.State;

using TMPro;

namespace STP.Behaviour.Meta.UI.FactionSystemWindow {
    public sealed class PlayerInventoryItemView : MonoBehaviour {
        public Image    ItemIcon;
        public TMP_Text ItemNameText;
        public TMP_Text ItemAmountText;
        public Button   Button;

        string                 _itemName;
        string                 _starSystemId;
        Action<string, string> _showInventoryItemSellWindow;
        InventoryItemInfos     _inventoryItemInfos;

        public void Init(string itemName, string starSystemId, Action<string, string> showInventoryItemSellWindow, InventoryItemInfos inventoryItemInfos) {
            _itemName                    = itemName;
            _starSystemId                = starSystemId;
            _showInventoryItemSellWindow = showInventoryItemSellWindow;
            _inventoryItemInfos          = inventoryItemInfos;
            
            Button.onClick.AddListener(OnClick);

            if ( !PlayerState.Instance.HasInInventory(itemName) ) {
                Debug.LogErrorFormat("No item '{0}' in player's inventory", itemName);
                return;
            }
            ItemNameText.text   = itemName;
            ItemAmountText.text = PlayerState.Instance.GetInventoryItemAmount(itemName).ToString();
            ItemIcon.sprite     = _inventoryItemInfos.GetItemInventoryIcon(itemName);
        }

        public void Deinit() {
            _itemName                    = null;
            _starSystemId                = null;
            _showInventoryItemSellWindow = null;

            Button.onClick.RemoveAllListeners();
        }

        void OnClick() {
            _showInventoryItemSellWindow?.Invoke(_itemName, _starSystemId);
        }
    }
}
