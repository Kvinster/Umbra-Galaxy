using UnityEngine;

using System;

namespace STP.State {
    public sealed class PlayerInventoryPlace {
        public string ItemName   { get; private set; } = string.Empty;
        public int    ItemAmount { get; private set; } = -1;

        public bool IsEmpty => string.IsNullOrEmpty(ItemName);

        public event Action<string, int> OnItemChanged;

        public void SetItem(string itemName, int itemAmount) {
            if ( (ItemName == itemName) && (ItemAmount == itemAmount) ) {
                return;
            }
            if ( !string.IsNullOrEmpty(itemName) ) {
                if ( !ItemNames.AllItems.Contains(itemName) ) {
                    Debug.LogErrorFormat("Unsupported item name '{0}'", itemName);
                    return;
                }
                if ( itemAmount <= 0 ) {
                    Debug.LogErrorFormat("Invalid item amount '{0}'", itemAmount);
                    return;
                }
            }
            ItemName   = itemName;
            ItemAmount = itemAmount;

            OnItemChanged?.Invoke(ItemName, ItemAmount);
        }
    }
}
