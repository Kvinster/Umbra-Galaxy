using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System;

using STP.Behaviour.Common;
using STP.State;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Meta.UI {
    public sealed class PlayerInventoryPlaceView : GameBehaviour, IPointerDownHandler, IPointerEnterHandler,
        IPointerExitHandler {
        [NotNull] public GameObject HasItemRoot;
        [NotNull] public Image      ItemIcon;

        bool _forceHide;

        InventoryItemInfos   _inventoryItemInfos;

        public bool ForceHide {
            get => _forceHide;
            set {
                if ( _forceHide == value ) {
                    return;
                }
                _forceHide = value;
                OnPlaceItemChanged(InventoryPlace.ItemName, InventoryPlace.ItemAmount);
            }
        }
        public PlayerInventoryPlace InventoryPlace { get; private set; }

        public event Action<PlayerInventoryPlaceView> OnClick;
        public event Action<PlayerInventoryPlaceView> OnHoverStart;
        public event Action<PlayerInventoryPlaceView> OnHoverFinish;

        public void Init(PlayerInventoryPlace inventoryPlace, InventoryItemInfos inventoryItemInfos) {
            InventoryPlace = inventoryPlace;
            
            _inventoryItemInfos = inventoryItemInfos;

            _forceHide = false;
            
            InventoryPlace.OnItemChanged += OnPlaceItemChanged;
            OnPlaceItemChanged(InventoryPlace.ItemName, InventoryPlace.ItemAmount);
        }
        
        public void Deinit() {
            if ( InventoryPlace != null ) {
                InventoryPlace.OnItemChanged -= OnPlaceItemChanged;
            }

            _forceHide = false;
            
            InventoryPlace = null;
            
            _inventoryItemInfos = null;
        }

        public void OnPointerDown(PointerEventData eventData) {
            OnClick?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            OnHoverStart?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData) {
            OnHoverFinish?.Invoke(this);
        }

        void OnPlaceItemChanged(string itemName, int itemAmount) {
            if ( string.IsNullOrEmpty(itemName) || ForceHide ) {
                HasItemRoot.SetActive(false);
                ItemIcon.sprite  = null;
                ItemIcon.enabled = false;
            } else {
                HasItemRoot.SetActive(true);
                ItemIcon.sprite  = _inventoryItemInfos.GetItemInventoryIcon(itemName);
                ItemIcon.enabled = true;
            }
        }
    }
}
