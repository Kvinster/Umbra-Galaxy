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
        
        string _starSystemId;
        
        InventoryItemInfos    _inventoryItemInfos;
        ProgressController    _progressController;
        StarSystemsController _starSystemsController;
        PlayerController      _playerController;

        public void Init(Action hide, InventoryItemInfos inventoryItemInfos, ProgressController progressController,
            StarSystemsController starSystemsController, PlayerController playerController) {
            base.Init(hide);
            _inventoryItemInfos    = inventoryItemInfos;
            _progressController    = progressController;
            _starSystemsController = starSystemsController;
            _playerController      = playerController;

            HideButton.onClick.AddListener(Hide);

            PlayerInventoryView.Init(_inventoryItemInfos, playerController);

            InventoryItemSell.Init(ShowInventorySellWindow, _inventoryItemInfos);
        }

        protected override void DeinitSpecific() {
            _starSystemId          = null;
            _inventoryItemInfos    = null;
            _progressController    = null;
            _starSystemsController = null;
            _playerController      = null;
            
            PlayerInventoryView.Deinit();

            InventoryItemSell.Deinit();
            
            HideButton.onClick.RemoveAllListeners();
        }

        public override void Show() {
            _starSystemId = _playerController.CurSystemId;
            
            PlayerNameText.text = string.Format(PlayerNameTextTemplate, "Player");
            UpdatePlayerMoneyText(_playerController.Money);

            var ssc = _starSystemsController;
            StarSystemPortrait.sprite  = ssc.GetFactionSystemPortrait(_starSystemId);
            StarSystemNameText.text    = string.Format(StarSystemNameTextTemplate, ssc.GetStarSystemName(_starSystemId));
            StarSystemFactionText.text = string.Format(StarSystemFactionTextTemplate,
                ssc.GetFactionSystemFaction(_starSystemId));
            StarSystemMoneyText.text =
                string.Format(StarSystemMoneyTextTemplate, ssc.GetFactionSystemMoney(_starSystemId));
            
            UpdateStarSystemMoneyText(ssc.GetFactionSystemMoney(_starSystemId));
            UpdateStarSystemSurvivalChanceText(ssc.GetFactionSystemSurvivalChance(_starSystemId));

            _playerController.OnMoneyChanged += OnPlayerMoneyChanged;

            ssc.OnStarSystemMoneyChanged          += OnStarSystemMoneyChanged;
            ssc.OnStarSystemSurvivalChanceChanged += OnStarSystemSurvivalChanceChanged;
        }

        protected override void HideSpecific() {
            _starSystemId = null;
            
            _playerController.OnMoneyChanged -= OnPlayerMoneyChanged;
            
            _starSystemsController.OnStarSystemMoneyChanged          -= OnStarSystemMoneyChanged;
            _starSystemsController.OnStarSystemSurvivalChanceChanged -= OnStarSystemSurvivalChanceChanged;
        }

        void ShowInventorySellWindow(PlayerInventoryPlace inventoryPlace) {
            WindowManager.Instance.Show<InventoryItemSellWindow>(x =>
                x.Init(inventoryPlace, _starSystemId, _inventoryItemInfos, _progressController,
                    _starSystemsController, _playerController));
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
