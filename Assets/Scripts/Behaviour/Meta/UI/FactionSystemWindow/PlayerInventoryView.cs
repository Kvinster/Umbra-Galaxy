using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Common;
using STP.State;

namespace STP.Behaviour.Meta.UI.FactionSystemWindow {
    public sealed class PlayerInventoryView : MonoBehaviour {
        public List<PlayerInventoryItemView> ItemViews = new List<PlayerInventoryItemView>();

        InventoryItemInfos _inventoryItemInfos;

        public event Action<string> OnItemClick;

        void Reset() {
            GetComponentsInChildren(ItemViews);
        }

        public void Init(InventoryItemInfos inventoryItemInfos) {
            _inventoryItemInfos = inventoryItemInfos;

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
                itemView.Init(playerInventoryEnum.Current.Key, _inventoryItemInfos);
                itemView.gameObject.SetActive(true);
                itemView.OnClick -= OnItemClickInternal;
                itemView.OnClick += OnItemClickInternal;
            }
            for ( ; itemViewIndex < ItemViews.Count; ++itemViewIndex ) {
                ItemViews[itemViewIndex].gameObject.SetActive(false);
            }
        }

        public void Deinit() {
            _inventoryItemInfos = null;

            ResetViews();

            foreach ( var itemView in ItemViews ) {
                itemView.OnClick -= OnItemClickInternal;
                itemView.Deinit();
            }
            
            PlayerState.Instance.OnInventoryChanged -= OnInventoryChanged;
        }

        void OnItemClickInternal(string itemName) {
            OnItemClick?.Invoke(itemName);
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
