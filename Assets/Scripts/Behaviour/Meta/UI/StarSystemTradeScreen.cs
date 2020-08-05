using UnityEngine;
using UnityEngine.UI;

using System;

using STP.Behaviour.Common;
using STP.Common.Windows;
using STP.State;
using STP.State.Meta;

using TMPro;

namespace STP.Behaviour.Meta.UI {
    public sealed class StarSystemTradeScreen : BaseStarSystemSubScreen {
        const string PlayerNameTextTemplate  = "Name: {0}";
        const string PlayerMoneyTextTemplate = "Money: {0}";
        
        const string StarSystemNameTextTemplate           = "Star System Name: {0}";
        const string StarSystemFactionTextTemplate        = "Faction: {0}";
        const string StarSystemMoneyTextTemplate          = "Money: {0}";
        const string StarSystemSurvivalChanceTextTemplate = "Survival Chance: {0}";

        static readonly string[] SurvivalChanceTexts = {
            "Zero",
            "Low",
            "Medium",
            "High",
            "Guaranteed"
        }; 

        public Image    PlayerPortrait;
        public TMP_Text PlayerNameText;
        public TMP_Text PlayerMoneyText;

        public Image    StarSystemPortrait;
        public TMP_Text StarSystemNameText;
        public TMP_Text StarSystemFactionText;
        public TMP_Text StarSystemMoneyText;
        public TMP_Text StarSystemSurvivalChanceText;

        public PlayerInventoryItemSell InventoryItemSell;
        
        public Button HideButton;

        public PlayerInventoryView PlayerInventoryView;
        
        string             _starSystemId;
        StarSystemsManager _starSystemsManager;
        InventoryItemInfos _inventoryItemInfos;

        public void Init(Action hide, StarSystemsManager starSystemsManager, InventoryItemInfos inventoryItemInfos) {
            _starSystemsManager = starSystemsManager;
            _inventoryItemInfos = inventoryItemInfos;

            HideButton.onClick.AddListener(() => {
                Hide();
                hide.Invoke();
            });

            PlayerInventoryView.Init(_inventoryItemInfos);

            InventoryItemSell.Init(ShowInventorySellWindow, _inventoryItemInfos);
        }

        public void Show(string starSystemId) {
            _starSystemId = starSystemId;

            var ps = PlayerState.Instance;
            PlayerNameText.text = string.Format(PlayerNameTextTemplate, "Player");
            UpdatePlayerMoneyText(ps.Money);

            var ssc = StarSystemsController.Instance;
            StarSystemPortrait.sprite  = ssc.GetFactionSystemPortrait(starSystemId);
            StarSystemNameText.text    = string.Format(StarSystemNameTextTemplate, ssc.GetStarSystemName(starSystemId));
            StarSystemFactionText.text = string.Format(StarSystemFactionTextTemplate,
                ssc.GetFactionSystemFaction(starSystemId));
            StarSystemMoneyText.text =
                string.Format(StarSystemMoneyTextTemplate, ssc.GetFactionSystemMoney(starSystemId));
            
            UpdateStarSystemMoneyText(StarSystemsController.Instance.GetFactionSystemMoney(_starSystemId));
            UpdateStarSystemSurvivalChanceText(ssc.GetFactionSystemSurvivalChance(_starSystemId));

            ps.OnMoneyChanged += OnPlayerMoneyChanged;

            StarSystemsController.Instance.OnStarSystemMoneyChanged          += OnStarSystemMoneyChanged;
            StarSystemsController.Instance.OnStarSystemSurvivalChanceChanged += OnStarSystemSurvivalChanceChanged;
        }

        public void Deinit() {
            _starSystemId       = null;
            _starSystemsManager = null;
            _inventoryItemInfos = null;
            
            PlayerInventoryView.Deinit();

            InventoryItemSell.Deinit();
            
            HideButton.onClick.RemoveAllListeners();
        }

        void Hide() {
            _starSystemId = null;

            var ps = PlayerState.Instance;
            ps.OnMoneyChanged -= OnPlayerMoneyChanged;

            var ssc = StarSystemsController.Instance;
            ssc.OnStarSystemMoneyChanged          -= OnStarSystemMoneyChanged;
            ssc.OnStarSystemSurvivalChanceChanged -= OnStarSystemSurvivalChanceChanged;
        }

        void ShowInventorySellWindow(PlayerInventoryPlace inventoryPlace) {
            WindowManager.Instance.Show<InventoryItemSellWindow>(x =>
                x.Init(inventoryPlace, _starSystemId, _starSystemsManager, _inventoryItemInfos));
        }

        void UpdatePlayerMoneyText(int playerMoney) {
            PlayerMoneyText.text = string.Format(PlayerMoneyTextTemplate, playerMoney);
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

        void OnPlayerMoneyChanged(int newMoney) {
            UpdatePlayerMoneyText(newMoney);
        }
    }
}
