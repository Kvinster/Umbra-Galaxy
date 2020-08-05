using UnityEngine;
using UnityEngine.UI;

using System;

using STP.Behaviour.Common;
using STP.State;

using TMPro;

namespace STP.Behaviour.Meta.UI {
    public sealed class PlayerInventoryItemSell : BasePlayerInventoryItemHandler {
        const string SellTextTemplate = "Sell {0} for {1}";

        public GameObject ActiveRoot;
        public TMP_Text   SellText;
        public Button     Button;

        Action<PlayerInventoryPlace> _showItemSellWindow;
        InventoryItemInfos           _inventoryItemInfos;

        protected override void OnPlayerInventoryViewRegistered() {
            CurPlayerInventoryView.OnSelectedPlaceViewChanged += OnSelectedPlayerInventoryPlaceViewChanged;
        }

        protected override void OnPlayerInventoryViewUnregistered(PlayerInventoryView playerInventoryView) {
            playerInventoryView.OnSelectedPlaceViewChanged -= OnSelectedPlayerInventoryPlaceViewChanged;
        }

        public void Init(Action<PlayerInventoryPlace> showItemSellWindow, InventoryItemInfos inventoryItemInfos) {
            _showItemSellWindow = showItemSellWindow;
            _inventoryItemInfos = inventoryItemInfos;

            Button.onClick.AddListener(OnClick);
            ActiveRoot.SetActive(false);
        }

        public void Deinit() {
            _showItemSellWindow = null;
            _inventoryItemInfos = null;

            Button.onClick.RemoveAllListeners();
        }

        void OnSelectedPlayerInventoryPlaceViewChanged(PlayerInventoryPlaceView placeView) {
            if ( !placeView || placeView.InventoryPlace.IsEmpty ) {
                ActiveRoot.SetActive(false);
            } else {
                ActiveRoot.SetActive(true);
                var itemName   = placeView.InventoryPlace.ItemName;
                var itemAmount = placeView.InventoryPlace.ItemAmount;
                SellText.text = string.Format(SellTextTemplate, itemName,
                    itemAmount * _inventoryItemInfos.GetItemBasePrice(itemName));
            }
        }

        void OnClick() {
            if ( !CurPlayerInventoryView || !CurPlayerInventoryView.SelectedPlaceView ||
                 CurPlayerInventoryView.SelectedPlaceView.InventoryPlace.IsEmpty ) {
                Debug.LogError("Unsupported scenario");
                return;
            }
            _showItemSellWindow?.Invoke(CurPlayerInventoryView.SelectedPlaceView.InventoryPlace);
            CurPlayerInventoryView.Deselect();
        }
    }
}
