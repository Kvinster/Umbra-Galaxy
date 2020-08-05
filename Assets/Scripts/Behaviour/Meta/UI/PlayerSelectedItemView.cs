using UnityEngine;
using UnityEngine.UI;

using STP.Behaviour.Common;

namespace STP.Behaviour.Meta.UI {
    public sealed class PlayerSelectedItemView : BasePlayerInventoryItemHandler {
        public GameObject SelectedItemViewRoot;
        public Image      SelectedItemIcon;

        InventoryItemInfos _inventoryItemInfos;

        Camera _camera;

        void Update() {
            var wp = _camera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(wp.x, wp.y, transform.position.z);
        }

        public void Init(InventoryItemInfos inventoryItemInfos) {
            _inventoryItemInfos = inventoryItemInfos;

            _camera = Camera.main;
        }

        public void Deinit() {
            Cursor.visible = true;
            _inventoryItemInfos = null;
            SelectedItemViewRoot.SetActive(false);
            
            _inventoryItemInfos = null;

            _camera = null;
        }
        
        protected override void OnPlayerInventoryViewRegistered() {
            CurPlayerInventoryView.OnSelectedPlaceViewChanged += OnSelectedPlaceViewChanged;
            OnSelectedPlaceViewChanged(CurPlayerInventoryView.SelectedPlaceView);
        }
        
        protected override void OnPlayerInventoryViewUnregistered(PlayerInventoryView playerInventoryView) {
            playerInventoryView.OnSelectedPlaceViewChanged -= OnSelectedPlaceViewChanged;
        }
        
        void OnSelectedPlaceViewChanged(PlayerInventoryPlaceView placeView) {
            SelectedItemViewRoot.SetActive(placeView);
            Cursor.visible = !placeView;
            SelectedItemIcon.sprite =
                placeView ? _inventoryItemInfos.GetItemInventoryIcon(placeView.InventoryPlace.ItemName) : null;
        }
    }
}
