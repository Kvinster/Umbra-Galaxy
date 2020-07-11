using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Common;
using STP.State;

namespace STP.Behaviour.Meta.UI.FactionSystemWindow {
    public sealed class PlayerInventoryView : MonoBehaviour {
        public List<PlayerInventoryItemView> ItemViews = new List<PlayerInventoryItemView>();

        string _starSystemId;

        void Reset() {
            GetComponentsInChildren(ItemViews);
        }

        public void CommonInit(MetaUiCanvas owner, InventoryItemInfos inventoryItemInfos) {
            foreach ( var itemView in ItemViews ) {
                itemView.CommonInit(inventoryItemInfos, owner);
                itemView.gameObject.SetActive(false);
            }
        }

        public void Init(string starSystemId) {
            _starSystemId = starSystemId;
            
            var playerInventoryEnum = PlayerState.Instance.GetInventoryEnumerator();
            var itemViewIndex       = 0;
            while ( playerInventoryEnum.MoveNext() ) {
                if ( itemViewIndex >= ItemViews.Count ) {
                    Debug.LogErrorFormat("Not enough PlayerInventoryItemViews: '{0}", ItemViews.Count);
                    break;
                }
                var itemView = ItemViews[itemViewIndex++];
                itemView.Init(playerInventoryEnum.Current.Key, starSystemId);
                itemView.gameObject.SetActive(true);
            }
            for ( ; itemViewIndex < ItemViews.Count; ++itemViewIndex ) {
                ItemViews[itemViewIndex].gameObject.SetActive(false);
            }

            PlayerState.Instance.OnInventoryChanged += OnInventoryChanged;
        }

        public void Deinit() {
            _starSystemId = null;
            
            ResetViews();
            PlayerState.Instance.OnInventoryChanged -= OnInventoryChanged;
        }

        void OnInventoryChanged() {
            Init(_starSystemId);
        }

        void ResetViews() {
            foreach ( var itemView in ItemViews ) {
                itemView.gameObject.SetActive(false);
            }
        }
    }
}
