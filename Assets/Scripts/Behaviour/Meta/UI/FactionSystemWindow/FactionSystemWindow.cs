using UnityEngine;
using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.State;

using TMPro;

namespace STP.Behaviour.Meta.UI.FactionSystemWindow {
    public sealed class FactionSystemWindow : MonoBehaviour {
        const string PlayerNameTextTemplate  = "Name: {0}";
        const string PlayerFuelTextTemplate  = "Fuel: {0}";
        const string PlayerMoneyTextTemplate = "Money: {0}";
        
        const string StarSystemNameTextTemplate    = "Star System Name: {0}";
        const string StarSystemFactionTextTemplate = "Faction: {0}";
        const string StarSystemMoneyTextTemplate   = "Money: {0}";
        const string RefillFuelButtonTextTemplate  = "Refill fuel ({0})";
        const string FuelFullText                  = "Fuel full";

        public Image    PlayerPortrait;
        public TMP_Text PlayerNameText;
        public TMP_Text PlayerFuelText;
        public TMP_Text PlayerMoneyText;

        public Image    StarSystemPortrait;
        public TMP_Text StarSystemNameText;
        public TMP_Text StarSystemFactionText;
        public TMP_Text StarSystemMoneyText;
        
        public TMP_Text FuelPriceText;
        public Button   RefillFuelButton;

        public Button HideWindowButton;

        public PlayerInventoryView PlayerInventoryView;
        
        MetaUiCanvas _owner;

        string _starSystemName;
        
        int _fuelPrice;

        bool CanSellFuel      => (PlayerState.Instance.Fuel < PlayerState.MaxFuel);
        bool PlayerCanBuyFuel => CanSellFuel && (PlayerState.Instance.Money >= _fuelPrice);
        
        public void CommonInit(MetaUiCanvas owner, MetaStarter starter) {
            _owner = owner;

            RefillFuelButton.onClick.AddListener(OnRefillFuelClick);
            HideWindowButton.onClick.AddListener(Hide);
            
            PlayerInventoryView.CommonInit(owner, starter.InventoryItemInfos);
        }

        public void Show(string starSystemName) {
            _starSystemName = starSystemName;
            
            PlayerInventoryView.Init(starSystemName);

            var ps = PlayerState.Instance;
            PlayerNameText.text = string.Format(PlayerNameTextTemplate, "Player");
            UpdatePlayerMoneyText(ps.Money);
            UpdatePlayerFuelText(ps.Fuel);

            var ssc = StarSystemsController.Instance;
            StarSystemPortrait.sprite  = ssc.GetStarSystemPortrait(starSystemName);
            StarSystemNameText.text    = string.Format(StarSystemNameTextTemplate, starSystemName);
            StarSystemFactionText.text = string.Format(StarSystemFactionTextTemplate,
                ssc.GetStarSystemFaction(starSystemName));
            StarSystemMoneyText.text =
                string.Format(StarSystemMoneyTextTemplate, ssc.GetStarSystemMoney(starSystemName));

            UpdateFuelPrice();
            UpdateRefillFuelButton();
            UpdateStarSystemMoneyText(StarSystemsController.Instance.GetStarSystemMoney(_starSystemName));

            ps.OnMoneyChanged += OnPlayerMoneyChanged;
            ps.OnFuelChanged  += OnPlayerFuelChanged;

            StarSystemsController.Instance.OnStarSystemMoneyChanged += OnStarSystemMoneyChanged;
        }

        void Hide() {
            _starSystemName = null;
            
            PlayerInventoryView.Deinit();
            
            PlayerState.Instance.OnMoneyChanged -= OnPlayerMoneyChanged;
            PlayerState.Instance.OnFuelChanged  -= OnPlayerFuelChanged;

            StarSystemsController.Instance.OnStarSystemMoneyChanged -= OnStarSystemMoneyChanged;

            _owner.HideFactionSystemWindow();
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
                _fuelPrice = (PlayerState.MaxFuel - PlayerState.Instance.Fuel) * 10;
                FuelPriceText.text = string.Format(RefillFuelButtonTextTemplate, _fuelPrice);
            } else {
                _fuelPrice = -1;
                FuelPriceText.text = FuelFullText;
            }
        }

        void OnStarSystemMoneyChanged(string starSystemName, int newMoney) {
            if ( _starSystemName == starSystemName ) {
                UpdateStarSystemMoneyText(newMoney);
            }
        }

        void UpdateStarSystemMoneyText(int money) {
            StarSystemMoneyText.text = string.Format(StarSystemMoneyTextTemplate, money);
        }

        void OnRefillFuelClick() {
            if ( PlayerCanBuyFuel ) {
                PlayerState.Instance.Money -= _fuelPrice;
                PlayerState.Instance.Fuel = PlayerState.MaxFuel;
            } else {
                Debug.LogError("Unsupported scenario");
            }
        }

        void OnPlayerMoneyChanged(int newMoney) {
            UpdatePlayerMoneyText(newMoney);
        }

        void OnPlayerFuelChanged(int newFuel) {
            UpdatePlayerFuelText(newFuel);
            UpdateFuelPrice();
            UpdateRefillFuelButton();
        }
    }
}
