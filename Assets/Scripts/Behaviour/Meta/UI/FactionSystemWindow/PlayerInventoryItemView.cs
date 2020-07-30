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

        string             _itemName;
        InventoryItemInfos _inventoryItemInfos;

        public event Action<string> OnClick;

        public void Init(string itemName, InventoryItemInfos inventoryItemInfos) {
            _itemName           = itemName;
            _inventoryItemInfos = inventoryItemInfos;
            
            Button.onClick.AddListener(OnClickInternal);

            if ( !PlayerState.Instance.HasInInventory(itemName) ) {
                Debug.LogErrorFormat("No item '{0}' in player's inventory", itemName);
                return;
            }
            ItemNameText.text   = itemName;
            ItemAmountText.text = PlayerState.Instance.GetInventoryItemAmount(itemName).ToString();
            ItemIcon.sprite     = _inventoryItemInfos.GetItemInventoryIcon(itemName);
        }

        public void Deinit() {
            _itemName = null;

            Button.onClick.RemoveAllListeners();
        }

        void OnClickInternal() {
            OnClick?.Invoke(_itemName);
        }
    }
}
