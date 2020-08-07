using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using STP.Behaviour.Common;
using STP.Behaviour.Utils;
using STP.Common;
using STP.Common.Windows;
using STP.State;
using STP.State.Meta;

using TMPro;

namespace STP.Behaviour.Meta.UI {
    public sealed class InventoryItemSellWindow : BaseWindow {
        const string ResultPriceTextTemplate = "Result price: {0}";
        
        public TMP_Text            ItemNameText;
        public Image               ItemIcon;
        public SliderTextFieldPair AmountPair;
        public TMP_Text            ResultPriceText;
        public Button              SellButton;
        public List<Button>        CloseWindowButtons;
        
        InventoryItemInfos    _inventoryItemInfos;
        ProgressController    _progressController;
        StarSystemsController _starSystemsController;
        PlayerController      _playerController;

        PlayerInventoryPlace _inventoryPlace;
        int                  _itemPrice;
        int                  _resultPrice;
        string               _starSystemId;

        bool CanSell => (_resultPrice <= _starSystemsController.GetFactionSystemMoney(_starSystemId));

        public void Init(PlayerInventoryPlace inventoryPlace, string starSystemId, 
            InventoryItemInfos inventoryItemInfos, ProgressController progressController,
            StarSystemsController starSystemsController, PlayerController playerController) {
            _inventoryPlace        = inventoryPlace;
            _starSystemId          = starSystemId;
            _inventoryItemInfos    = inventoryItemInfos;
            _progressController    = progressController;
            _starSystemsController = starSystemsController;
            _playerController      = playerController;

            var itemName   = _inventoryPlace.ItemName;
            var itemAmount = _inventoryPlace.ItemAmount;

            ItemNameText.text = itemName;
            ItemIcon.sprite   = _inventoryItemInfos.GetItemInventoryIcon(itemName);

            AmountPair.CommonInit();
            AmountPair.Init(1, itemAmount);

            SellButton.onClick.AddListener(OnSellClick);
            foreach ( var closeWindowButton in CloseWindowButtons ) {
                closeWindowButton.onClick.AddListener(Hide);
            }

            _itemPrice = _inventoryItemInfos.GetItemBasePrice(itemName);

            AmountPair.OnValueChanged += OnAmountChanged;

            UpdateResultPrice(AmountPair.CurValue);
        }

        protected override void Deinit() {
            _inventoryPlace        = null;
            _starSystemId          = null;
            _inventoryItemInfos    = null;
            _progressController    = null;
            _starSystemsController = null;
            _playerController      = null;

            AmountPair.OnValueChanged -= OnAmountChanged;
            AmountPair.Deinit();

            SellButton.onClick.RemoveAllListeners();
            foreach ( var closeWindowButton in CloseWindowButtons ) {
                closeWindowButton.onClick.RemoveAllListeners();
            }
        }

        void OnAmountChanged(int newAmount) {
            UpdateResultPrice(newAmount);
        }

        void UpdateResultPrice(int amount) {
            _resultPrice = _itemPrice * amount;
            ResultPriceText.text = string.Format(ResultPriceTextTemplate, _resultPrice);
            SellButton.interactable = CanSell;
        }

        void OnSellClick() {
            if ( !CanSell ) {
                Debug.LogErrorFormat("Unsupported scenario");
                return;
            }
            var itemName       = _inventoryPlace.ItemName;
            var sellItemAmount = AmountPair.CurValue;
            var newItemAmount  = _inventoryPlace.ItemAmount - sellItemAmount;
            if ( _starSystemsController.TrySubFactionSystemMoney(_starSystemId, _resultPrice) ) {
                _inventoryPlace.SetItem((newItemAmount == 0) ? string.Empty : itemName, newItemAmount);
                _playerController.Money += _resultPrice;
                _starSystemsController.AddFactionSystemSurvivalChance(_starSystemId,
                    _inventoryItemInfos.GetItemBaseSurvivalChanceInc(itemName) * sellItemAmount);
                
                if ( _starSystemsController.GetStarSystemType(_starSystemId) == StarSystemType.Faction ) {
                    _progressController.OnSellItemToFaction(itemName, sellItemAmount,
                        _starSystemsController.GetFactionSystemFaction(_starSystemId));
                }
                
                Hide();
            }
        }
    }
}
