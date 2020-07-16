using UnityEngine;
using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.State;
using STP.State.Meta;

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

        string _starSystemId;
        
        int _fuelPrice;
        int _fuelAmount;

        bool CanSellFuel      => (PlayerState.Instance.Fuel < PlayerState.MaxFuel);
        bool PlayerCanBuyFuel => CanSellFuel && (PlayerState.Instance.Money >= _fuelPrice);
        
        public void CommonInit(MetaUiCanvas owner, MetaStarter starter) {
            _owner = owner;

            RefillFuelButton.onClick.AddListener(OnRefillFuelClick);
            HideWindowButton.onClick.AddListener(Hide);
            
            PlayerInventoryView.CommonInit(owner, starter.InventoryItemInfos);
        }

        public void Show(string starSystemId) {
            _starSystemId = starSystemId;
            
            PlayerInventoryView.Init(starSystemId);

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

            ps.OnMoneyChanged += OnPlayerMoneyChanged;
            ps.OnFuelChanged  += OnPlayerFuelChanged;

            StarSystemsController.Instance.OnStarSystemMoneyChanged += OnStarSystemMoneyChanged;
        }

        void Hide() {
            _starSystemId = null;
            
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

        void UpdateStarSystemMoneyText(int money) {
            StarSystemMoneyText.text = string.Format(StarSystemMoneyTextTemplate, money);
        }

        void OnRefillFuelClick() {
            if ( PlayerCanBuyFuel ) {
                PlayerState.Instance.Money -= _fuelPrice;
                PlayerState.Instance.Fuel  += _fuelAmount;
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
