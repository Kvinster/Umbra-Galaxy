﻿using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using STP.Behaviour.Common;
using STP.Behaviour.Utils;
using STP.Common;
using STP.State;
using STP.State.Meta;

using TMPro;

namespace STP.Behaviour.Meta.UI {
    public sealed class InventoryItemSellWindow : MonoBehaviour {
        const string ResultPriceTextTemplate = "Result price: {0}";
        
        public TMP_Text            ItemNameText;
        public Image               ItemIcon;
        public SliderTextFieldPair AmountPair;
        public TMP_Text            ResultPriceText;
        public Button              SellButton;
        public List<Button>        CloseWindowButtons;

        StarSystemsManager _starSystemsManager;
        MetaUiCanvas       _owner;
        InventoryItemInfos _inventoryItemInfos;

        string _itemName;
        int    _itemPrice;
        int    _resultPrice;
        string _starSystemId;

        bool CanSell => (_resultPrice <= StarSystemsController.Instance.GetFactionSystemMoney(_starSystemId));

        public void CommonInit(StarSystemsManager starSystemsManager, MetaUiCanvas owner,
            InventoryItemInfos inventoryItemInfos) {
            _starSystemsManager = starSystemsManager;
            _owner              = owner;
            _inventoryItemInfos = inventoryItemInfos;

            AmountPair.CommonInit();

            SellButton.onClick.AddListener(OnSellClick);
            foreach ( var closeWindowButton in CloseWindowButtons ) {
                closeWindowButton.onClick.AddListener(Hide);
            }
        }

        public void Show(string itemName, string starSystemId) {
            _itemName     = itemName;
            _starSystemId = starSystemId;
            
            ItemNameText.text = itemName;
            ItemIcon.sprite   = _inventoryItemInfos.GetItemInventoryIcon(itemName);

            var inventoryAmount = PlayerState.Instance.GetInventoryItemAmount(_itemName);
            AmountPair.Init(1, inventoryAmount);

            _itemPrice = _inventoryItemInfos.GetItemBasePrice(itemName);
            
            AmountPair.OnValueChanged += OnAmountChanged;

            UpdateResultPrice(AmountPair.CurValue);
        }

        void Hide() {
            _itemName     = null;
            _starSystemId = null;
            
            AmountPair.OnValueChanged -= OnAmountChanged;
            AmountPair.Deinit();

            _owner.HideInventoryItemSellWindow();
        }

        void UpdateAmount(int inventoryAmount) {
            if ( inventoryAmount <= 0 ) {
                Debug.LogErrorFormat("Invalid inventory item '{0}' amount: '{1}'", _itemName, inventoryAmount);
                return;
            }
            AmountPair.SetBorderValues(1, inventoryAmount);
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
            var ps     = PlayerState.Instance;
            var ssc    = StarSystemsController.Instance;
            var amount = AmountPair.CurValue;
            if ( ps.TryTakeFromInventory(_itemName, amount) &&
                 ssc.TrySubFactionSystemMoney(_starSystemId, _resultPrice) ) {
                ps.Money += _resultPrice;
                ssc.AddFactionSystemSurvivalChance(_starSystemId,
                    _inventoryItemInfos.GetItemBaseSurvivalChanceInc(_itemName) * amount);
                
                if ( _starSystemsManager.GetStarSystem(_starSystemId).Type == StarSystemType.Faction ) {
                    ProgressController.Instance.OnSellItemToFaction(_itemName, amount,
                        ssc.GetFactionSystemFaction(_starSystemId));
                }

                var inventoryAmount = ps.GetInventoryItemAmount(_itemName);
                if ( inventoryAmount <= 0 ) {
                    Hide();
                } else {
                    UpdateAmount(inventoryAmount);
                }
            }
        }
    }
}
