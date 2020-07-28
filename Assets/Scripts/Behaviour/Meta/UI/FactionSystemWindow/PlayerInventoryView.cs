using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Common;
using STP.State;

namespace STP.Behaviour.Meta.UI.FactionSystemWindow {
    public sealed class PlayerInventoryView : MonoBehaviour {
        public List<PlayerInventoryItemView> ItemViews = new List<PlayerInventoryItemView>();

        string                 _starSystemId;
        InventoryItemInfos     _inventoryItemInfos;
        Action<string, string> _showInventoryItemSellWindow;

        void Reset() {
            GetComponentsInChildren(ItemViews);
        }

        public void Init(string starSystemId, InventoryItemInfos inventoryItemInfos, Action<string, string> showInventoryItemSellWindow) {
            _starSystemId                = starSystemId;
            _inventoryItemInfos          = inventoryItemInfos;
            _showInventoryItemSellWindow = showInventoryItemSellWindow;

            PlayerState.Instance.OnInventoryChanged -= OnInventoryChanged;
            PlayerState.Instance.OnInventoryChanged += OnInventoryChanged;

            UpdateView();
        }

        void UpdateView() {
            var playerInventoryEnum = PlayerState.Instance.GetInventoryEnumerator();
            var itemViewIndex       = 0;
            while ( playerInventoryEnum.MoveNext() ) {
                if ( itemViewIndex >= ItemViews.Count ) {
                    Debug.LogErrorFormat("Not enough PlayerInventoryItemViews: '{0}", ItemViews.Count);
                    break;
                }
                var itemView = ItemViews[itemViewIndex++];
                itemView.Deinit();
                itemView.Init(playerInventoryEnum.Current.Key, _starSystemId, _showInventoryItemSellWindow,
                    _inventoryItemInfos);
                itemView.gameObject.SetActive(true);
            }
            for ( ; itemViewIndex < ItemViews.Count; ++itemViewIndex ) {
                ItemViews[itemViewIndex].gameObject.SetActive(false);
            }
        }

        public void Deinit() {
            _starSystemId                = null;
            _inventoryItemInfos          = null;
            _showInventoryItemSellWindow = null;

            ResetViews();

            foreach ( var itemView in ItemViews ) {
                itemView.Deinit();
            }
            
            PlayerState.Instance.OnInventoryChanged -= OnInventoryChanged;
        }

        void OnInventoryChanged() {
            UpdateView();
        }

        void ResetViews() {
            foreach ( var itemView in ItemViews ) {
                itemView.gameObject.SetActive(false);
            }
        }
    }
}
