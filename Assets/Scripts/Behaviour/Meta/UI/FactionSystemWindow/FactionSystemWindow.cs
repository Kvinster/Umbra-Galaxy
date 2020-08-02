using UnityEngine;
using UnityEngine.UI;

using STP.Behaviour.Common;
using STP.Common.Windows;
using STP.State;
using STP.State.Meta;

using TMPro;

namespace STP.Behaviour.Meta.UI.FactionSystemWindow {
    public sealed class FactionSystemWindow : BaseWindow {
        const string PlayerNameTextTemplate  = "Name: {0}";
        const string PlayerFuelTextTemplate  = "Fuel: {0}";
        const string PlayerMoneyTextTemplate = "Money: {0}";
        
        const string StarSystemNameTextTemplate           = "Star System Name: {0}";
        const string StarSystemFactionTextTemplate        = "Faction: {0}";
        const string StarSystemMoneyTextTemplate          = "Money: {0}";
        const string StarSystemSurvivalChanceTextTemplate = "Survival Chance: {0}";
        const string RefillFuelButtonTextTemplate         = "Refill fuel ({0})";
        const string FuelFullText                         = "Fuel full";

        static readonly string[] SurvivalChanceTexts = {
            "Zero",
            "Low",
            "Medium",
            "High",
            "Guaranteed"
        }; 

        public Image    PlayerPortrait;
        public TMP_Text PlayerNameText;
        public TMP_Text PlayerFuelText;
        public TMP_Text PlayerMoneyText;

        public Image    StarSystemPortrait;
        public TMP_Text StarSystemNameText;
        public TMP_Text StarSystemFactionText;
        public TMP_Text StarSystemMoneyText;
        public TMP_Text StarSystemSurvivalChanceText;

        public PlayerInventoryItemSell InventoryItemSell;
        
        public TMP_Text FuelPriceText;
        public Button   RefillFuelButton;

        public PlayerInventoryView PlayerInventoryView;

        string             _starSystemId;
        StarSystemsManager _starSystemsManager;
        InventoryItemInfos _inventoryItemInfos;
        
        int _fuelPrice;
        int _fuelAmount;

        bool CanSellFuel      => (PlayerState.Instance.Fuel < PlayerState.MaxFuel);
        bool PlayerCanBuyFuel => CanSellFuel && (PlayerState.Instance.Money >= _fuelPrice);

        public void Init(string starSystemId, StarSystemsManager starSystemsManager,
            InventoryItemInfos inventoryItemInfos) {
            _starSystemId       = starSystemId;
            _starSystemsManager = starSystemsManager;
            _inventoryItemInfos = inventoryItemInfos;

            RefillFuelButton.onClick.AddListener(OnRefillFuelClick);

            PlayerInventoryView.Init(inventoryItemInfos);

            InventoryItemSell.Init(ShowInventorySellWindow, inventoryItemInfos);

            var ps = PlayerState.Instance;
            PlayerNameText.text = string.Format(PlayerNameTextTemplate, "Player");
            UpdatePlayerMoneyText(ps.Money);
            UpdatePlayerFuelText(ps.Fuel);

            var ssc = StarSystemsController.Instance;
            StarSystemPortrait.sprite  = ssc.GetFactionSystemPortrait(starSystemId);
            StarSystemNameText.text    = string.Format(StarSystemNameTextTemplate, ssc.GetStarSystemName(starSystemId));
            StarSystemFactionText.text = string.Format(StarSystemFactionTextTemplate,
                ssc.GetFactionSystemFaction(starSystemId));
            StarSystemMoneyText.text =
                string.Format(StarSystemMoneyTextTemplate, ssc.GetFactionSystemMoney(starSystemId));

            UpdateFuelPrice();
            UpdateRefillFuelButton();
            UpdateStarSystemMoneyText(StarSystemsController.Instance.GetFactionSystemMoney(_starSystemId));
            UpdateStarSystemSurvivalChanceText(ssc.GetFactionSystemSurvivalChance(_starSystemId));

            ps.OnMoneyChanged += OnPlayerMoneyChanged;
            ps.OnFuelChanged  += OnPlayerFuelChanged;

            StarSystemsController.Instance.OnStarSystemMoneyChanged          += OnStarSystemMoneyChanged;
            StarSystemsController.Instance.OnStarSystemSurvivalChanceChanged += OnStarSystemSurvivalChanceChanged;
        }

        protected override void Deinit() {
            _starSystemId       = null;
            _starSystemsManager = null;
            _inventoryItemInfos = null;
            
            PlayerInventoryView.Deinit();

            InventoryItemSell.Deinit();
            
            RefillFuelButton.onClick.RemoveAllListeners();
            
            PlayerState.Instance.OnMoneyChanged -= OnPlayerMoneyChanged;
            PlayerState.Instance.OnFuelChanged  -= OnPlayerFuelChanged;

            StarSystemsController.Instance.OnStarSystemMoneyChanged          -= OnStarSystemMoneyChanged;
            StarSystemsController.Instance.OnStarSystemSurvivalChanceChanged -= OnStarSystemSurvivalChanceChanged;
        }

        void ShowInventorySellWindow(PlayerInventoryPlace inventoryPlace) {
            WindowManager.Instance.Show<InventoryItemSellWindow>(x =>
                x.Init(inventoryPlace, _starSystemId, _starSystemsManager, _inventoryItemInfos));
        }

        void UpdatePlayerMoneyText(int playerMoney) {
            PlayerMoneyText.text = string.Format(PlayerMoneyTextTemplate, playerMoney);
        }

        void UpdatePlayerFuelText(int playerFuel) {
            PlayerFuelText.text = string.Format(PlayerFuelTextTemplate, playerFuel);
        }

        void UpdateRefillFuelButton() {
            RefillFuelButton.interactable = PlayerCanBuyFuel;
        }

        void UpdateFuelPrice() {
            if ( CanSellFuel ) {
                var ps         = PlayerState.Instance;
                var playerFuel = ps.Fuel;
                _fuelPrice = (PlayerState.MaxFuel - playerFuel) * 10;
                if ( ps.Money < _fuelPrice ) {
                    _fuelAmount = ps.Money / 10;
                    _fuelPrice  = _fuelAmount * 10;
                } else {
                    _fuelAmount = PlayerState.MaxFuel - playerFuel;
                }
                FuelPriceText.text = string.Format(RefillFuelButtonTextTemplate, _fuelPrice);
            } else {
                _fuelPrice = -1;
                FuelPriceText.text = FuelFullText;
            }
        }

        void OnStarSystemMoneyChanged(string starSystemId, int newMoney) {
            if ( _starSystemId == starSystemId ) {
                UpdateStarSystemMoneyText(newMoney);
            }
        }

        void OnStarSystemSurvivalChanceChanged(string starSystemId, int newSurvivalChance) {
            if ( _starSystemId == starSystemId ) {
                UpdateStarSystemSurvivalChanceText(newSurvivalChance);
            }
        }

        void UpdateStarSystemMoneyText(int money) {
            StarSystemMoneyText.text = string.Format(StarSystemMoneyTextTemplate, money);
        }

        void UpdateStarSystemSurvivalChanceText(int survivalChance) {
            int index;
            if ( survivalChance == 0 ) {
                index = 0;
            } else if ( survivalChance < 30 ) {
                index = 1;
            } else if ( survivalChance < 70 ) {
                index = 2;
            } else if ( survivalChance < 90 ) {
                index = 3;
            } else if ( survivalChance < 100) {
                index = 4;
            } else {
                index = 5;
            }
            index = Mathf.Clamp(index, 0, SurvivalChanceTexts.Length - 1);
            StarSystemSurvivalChanceText.text =
                string.Format(StarSystemSurvivalChanceTextTemplate, SurvivalChanceTexts[index]);
        }

        void OnRefillFuelClick() {
            if ( PlayerCanBuyFuel ) {
                var price  = _fuelPrice;
                var amount = _fuelAmount;
                PlayerState.Instance.Money -= price;
                PlayerState.Instance.Fuel  += amount;
            } else {
                Debug.LogError("Unsupported scenario");
            }
        }

        void OnPlayerMoneyChanged(int newMoney) {
            UpdatePlayerMoneyText(newMoney);
            UpdateFuelPrice();
            UpdateRefillFuelButton();
        }

        void OnPlayerFuelChanged(int newFuel) {
            UpdatePlayerFuelText(newFuel);
            UpdateFuelPrice();
            UpdateRefillFuelButton();
        }
    }
}
