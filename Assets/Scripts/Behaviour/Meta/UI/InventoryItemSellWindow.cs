using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using STP.Behaviour.Common;
using STP.Behaviour.Utils;
using STP.State;

using TMPro;

namespace STP.Behaviour.Meta.UI {
    public sealed class InventoryItemSellWindow : MonoBehaviour {
        const string ResultPriceTextTemplate = "Result price: {0}";
        
        public TMP_Text            ItemNameText;
        public Image               ItemIcon;
        public SliderTextFieldPair PricePair;
        public SliderTextFieldPair AmountPair;
        public TMP_Text            ResultPriceText;
        public Button              SellButton;
        public List<Button>        CloseWindowButtons;

        MetaUiCanvas     _owner;
        InventoryItemInfos _inventoryItemInfos;

        string _itemName;
        int    _resultPrice;
        string _starSystemName;

        bool CanSell => (_resultPrice <= StarSystemsController.Instance.GetFactionSystemMoney(_starSystemName));

        public void CommonInit(MetaUiCanvas owner, InventoryItemInfos inventoryItemInfos) {
            _owner              = owner;
            _inventoryItemInfos = inventoryItemInfos;

            AmountPair.CommonInit();
            PricePair.CommonInit();

            SellButton.onClick.AddListener(OnSellClick);
            foreach ( var closeWindowButton in CloseWindowButtons ) {
                closeWindowButton.onClick.AddListener(Hide);
            }
        }

        public void Show(string itemName, string starSystemName) {
            _itemName       = itemName;
            _starSystemName = starSystemName;
            
            ItemNameText.text = itemName;
            ItemIcon.sprite   = _inventoryItemInfos.GetItemInventoryIcon(itemName);

            var inventoryAmount = PlayerState.Instance.GetInventoryItemAmount(_itemName);
            AmountPair.Init(1, inventoryAmount);

            var basePrice = _inventoryItemInfos.GetItemBasePrice(itemName);
            PricePair.Init(Mathf.FloorToInt(basePrice * 0.9f), Mathf.CeilToInt(basePrice * 1.1f));
            
            PricePair.OnValueChanged  += OnPriceChanged;
            AmountPair.OnValueChanged += OnAmountChanged;

            UpdateResultPrice(PricePair.CurValue, AmountPair.CurValue);
        }

        void Hide() {
            _itemName       = null;
            _starSystemName = null;
            
            PricePair.OnValueChanged  -= OnPriceChanged;
            PricePair.Deinit();
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

        void OnPriceChanged(int newPrice) {
            UpdateResultPrice(newPrice, AmountPair.CurValue);
        }

        void OnAmountChanged(int newAmount) {
            UpdateResultPrice(PricePair.CurValue, newAmount);
        }

        void UpdateResultPrice(int price, int amount) {
            _resultPrice = price * amount;
            ResultPriceText.text = string.Format(ResultPriceTextTemplate, _resultPrice);
            SellButton.interactable = CanSell;
        }

        void OnSellClick() {
            if ( !CanSell ) {
                Debug.LogErrorFormat("Unsupported scenario");
                return;
            }
            if ( PlayerState.Instance.TryTakeFromInventory(_itemName, AmountPair.CurValue) &&
                 StarSystemsController.Instance.TrySubFactionSystemMoney(_starSystemName, _resultPrice) ) {
                PlayerState.Instance.Money += _resultPrice;

                var inventoryAmount = PlayerState.Instance.GetInventoryItemAmount(_itemName);
                if ( inventoryAmount <= 0 ) {
                    Hide();
                } else {
                    UpdateAmount(inventoryAmount);
                }
            }
        }
    }
}
