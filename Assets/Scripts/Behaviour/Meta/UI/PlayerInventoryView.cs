using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Common;
using STP.Behaviour.Utils;
using STP.State;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.Meta.UI {
    public sealed class PlayerInventoryView : GameComponent {
        [NotNull]
        public PlayerSelectedItemView               SelectedItemView;
        [NotNull]
        public HoverTooltip                         Tooltip;
        [NotNull]
        public TMP_Text                             TooltipText;
        [NotNullOrEmpty]
        public List<PlayerInventoryPlaceView>       PlaceViews   = new List<PlayerInventoryPlaceView>();
        public List<BasePlayerInventoryItemHandler> ItemHandlers = new List<BasePlayerInventoryItemHandler>();

        PlayerInventoryPlaceView _selectedPlaceView;
        public PlayerInventoryPlaceView SelectedPlaceView {
            get => _selectedPlaceView;
            private set {
                if ( _selectedPlaceView == value ) {
                    return;
                }
                _selectedPlaceView = value;
                OnSelectedPlaceViewChanged?.Invoke(_selectedPlaceView);
            }
        }

        public event Action<PlayerInventoryPlaceView> OnSelectedPlaceViewChanged; 

        public PlayerInventoryPlace SelectedInventoryPlace =>
            SelectedPlaceView ? SelectedPlaceView.InventoryPlace : null;

        void Reset() {
            GetComponentsInChildren(PlaceViews);
        }

        protected override void CheckDescription() {
            if ( (PlaceViews == null) || (PlaceViews.Count < PlayerInventory.InventoryPlaces) ) {
                Debug.LogError("Not enough PlayerInventoryPlaceViews");
            }
        }

        public void Init(InventoryItemInfos inventoryItemInfos, PlayerController playerController) {
            SelectedItemView.Init(inventoryItemInfos);
            SelectedItemView.RegisterPlayerInventoryView(this);
            var placeViewIndex  = 0;
            var playerInventory = playerController.Inventory;
            for ( var i = 0; i < PlayerInventory.InventoryPlaces; ++i ) {
                var placeView = PlaceViews[placeViewIndex++];
                placeView.Init(playerInventory.GetPlace(i), inventoryItemInfos);
                placeView.OnClick       += TrySelect;
                placeView.OnHoverStart  += OnHoverStart;
                placeView.OnHoverFinish += OnHoverFinish;
                placeView.gameObject.SetActive(true);
            }
            for ( ; placeViewIndex < PlaceViews.Count; ++placeViewIndex ) {
                PlaceViews[placeViewIndex].gameObject.SetActive(false);
            }
            foreach ( var handler in ItemHandlers ) {
                handler.RegisterPlayerInventoryView(this);
            }
            SelectedPlaceView = null;
        }

        public void Deinit() {
            SelectedItemView.UnregisterPlayerInventoryView(this);
            SelectedItemView.Deinit();
            foreach ( var placeView in PlaceViews ) {
                placeView.OnClick       -= TrySelect;
                placeView.OnHoverStart  -= OnHoverStart;
                placeView.OnHoverFinish -= OnHoverFinish;
                placeView.Deinit();
            }
            foreach ( var handler in ItemHandlers ) {
                handler.UnregisterPlayerInventoryView(this);
            }
            SelectedPlaceView = null;
        }

        public void Deselect() {
            if ( SelectedPlaceView != null ) {
                SelectedPlaceView.ForceHide = false;
            }
            SelectedPlaceView = null;
        }

        void TrySelect(PlayerInventoryPlaceView placeView) {
            if ( SelectedPlaceView ) {
                SelectedPlaceView.ForceHide = false;
            }
            if ( placeView == SelectedPlaceView ) {
                SelectedPlaceView = null;
                return;
            }
            if ( placeView.InventoryPlace.IsEmpty ) {
                if ( SelectedPlaceView ) {
                    var oldPlace = SelectedPlaceView.InventoryPlace;
                    var itemName = oldPlace.ItemName;
                    var itemAmount = oldPlace.ItemAmount;
                    oldPlace.SetItem(string.Empty, 0);
                    var newPlace = placeView.InventoryPlace;
                    newPlace.SetItem(itemName, itemAmount);
                    SelectedPlaceView = null;

                    OnHoverStart(placeView);
                }
            } else {
                SelectedPlaceView = placeView;
                SelectedPlaceView.ForceHide = true;
                
                if ( Tooltip ) {
                    Tooltip.Hide();
                }
            }
        }

        void OnHoverStart(PlayerInventoryPlaceView placeView) {
            if ( !Tooltip || SelectedPlaceView || (placeView.InventoryPlace == null) ||
                 (placeView.InventoryPlace.IsEmpty) ) {
                return;
            }
            var itemName   = placeView.InventoryPlace.ItemName;
            var itemAmount = placeView.InventoryPlace.ItemAmount;
            TooltipText.text = (itemAmount == 1) ? itemName : $"{itemName} ({itemAmount})";
            Tooltip.Show();
        }

        void OnHoverFinish(PlayerInventoryPlaceView placeView) {
            if ( !Tooltip ) {
                return;
            }
            Tooltip.Hide();
        }
    }
}
