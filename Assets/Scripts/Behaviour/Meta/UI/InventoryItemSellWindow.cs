﻿using UnityEngine;
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

        StarSystemsManager _starSystemsManager;
        InventoryItemInfos _inventoryItemInfos;

        PlayerInventoryPlace _inventoryPlace;
        int                  _itemPrice;
        int                  _resultPrice;
        string               _starSystemId;

        bool CanSell => (_resultPrice <= StarSystemsController.Instance.GetFactionSystemMoney(_starSystemId));

        public void Init(PlayerInventoryPlace inventoryPlace, string starSystemId,
            StarSystemsManager starSystemsManager, InventoryItemInfos inventoryItemInfos) {
            _starSystemsManager = starSystemsManager;
            _inventoryItemInfos = inventoryItemInfos;
            _inventoryPlace     = inventoryPlace;
            _starSystemId       = starSystemId;

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
            _starSystemsManager = null;
            _inventoryItemInfos = null;
            _inventoryPlace     = null;
            _starSystemId       = null;
            
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
            var ps             = PlayerState.Instance;
            var ssc            = StarSystemsController.Instance;
            var itemName       = _inventoryPlace.ItemName;
            var sellItemAmount = AmountPair.CurValue;
            var newItemAmount  = _inventoryPlace.ItemAmount - sellItemAmount;
            if ( ssc.TrySubFactionSystemMoney(_starSystemId, _resultPrice) ) {
                _inventoryPlace.SetItem(itemName, newItemAmount);
                ps.Money += _resultPrice;
                ssc.AddFactionSystemSurvivalChance(_starSystemId,
                    _inventoryItemInfos.GetItemBaseSurvivalChanceInc(itemName) * sellItemAmount);
                
                if ( _starSystemsManager.GetStarSystem(_starSystemId).Type == StarSystemType.Faction ) {
                    ProgressController.Instance.OnSellItemToFaction(itemName, sellItemAmount,
                        ssc.GetFactionSystemFaction(_starSystemId));
                }
                
                Hide();
            }
        }
    }
}
