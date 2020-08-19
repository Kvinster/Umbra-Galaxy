using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

using STP.State;

namespace STP.Behaviour.Common {
    [CreateAssetMenu(menuName = "Create ItemsSpriteSetup", fileName = "InventoryItemInfos")]
    public sealed class InventoryItemInfos : ScriptableObject {
        [Serializable]
        public sealed class ItemInfo {
            public string ItemName;
            public int    ItemBasePrice;
            public int    ItemBaseSurvivalChanceInc;
            public Sprite ItemInventoryIcon;
        }

        public List<ItemInfo> ItemInfos = new List<ItemInfo>();

        public Sprite GetItemInventoryIcon(string itemName) {
            return TryGetItemInfo(itemName, out var itemInfo) ? itemInfo.ItemInventoryIcon : null;
        }

        public int GetItemBasePrice(string itemName) {
            return TryGetItemInfo(itemName, out var itemInfo) ? itemInfo.ItemBasePrice : -1;
        }

        public int GetItemBaseSurvivalChanceInc(string itemName) {
            return TryGetItemInfo(itemName, out var itemInfo) ? itemInfo.ItemBaseSurvivalChanceInc : -1;
        }

        bool TryGetItemInfo(string itemName, out ItemInfo itemInfo) {
            foreach ( var pack in ItemInfos ) {
                if ( pack.ItemName == itemName ) {
                    itemInfo = pack;
                    return true;
                }
            }
            Debug.LogErrorFormat("Can't find ItemInfo for item '{0}'", itemName);
            itemInfo = null;
            return false;
        }

        [ContextMenu("Populate from ItemNames")]
        void PopulateFromItemNames() {
            ItemInfos.RemoveAll(x => !ItemNames.AllItems.Contains(x.ItemName));
            var newNames = ItemNames.AllItems.Where(x => !ItemInfos.Exists(y => y.ItemName == x));
            foreach ( var itemName in newNames ) {
                ItemInfos.Add(new ItemInfo {
                    ItemName          = itemName,
                    ItemBasePrice     = 0,
                    ItemInventoryIcon = null
                });
            }
        }
    }
}
