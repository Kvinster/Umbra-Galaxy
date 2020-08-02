using UnityEngine;
using UnityEngine.UI;

namespace STP.Behaviour.Meta.UI {
    public sealed class PlayerInventoryItemTrash : BasePlayerInventoryItemHandler {
        public Button Button;

        void Start() {
            Button.onClick.AddListener(OnClick);
        }

        void OnClick() {
            if ( !CurPlayerInventoryView ) {
                Debug.LogError("No PlayerInventoryView registered");
                return;
            }
            var inventoryPlace = CurPlayerInventoryView.SelectedInventoryPlace;
            if ( inventoryPlace == null ) {
                return;
            }
            inventoryPlace.SetItem(string.Empty, 0);
            CurPlayerInventoryView.Deselect();
        }
    }
}
