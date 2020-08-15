using UnityEngine;

using System;

using STP.Behaviour.Utils;
using STP.State;

using TMPro;

namespace STP.Behaviour.Meta.UI {
    public sealed class StarSystemHangarScreen : BaseStarSystemSubScreen {
        public TMP_Text     TooltipText;
        public HoverTooltip Tooltip;
        [Space]
        public HoverButton RefuelButton;
        public HoverButton RepairButton;
        public HoverButton TakeoffButton;
        public HoverButton HideButton;

        PlayerController _playerController;

        int _fuelPrice;
        int _fuelAmount;

        int _repairPrice;
        int _repairAmount;
        
        StarSystemUiManager _owner;

        public void Init(Action hide, StarSystemUiManager owner, PlayerController playerController) {
            base.Init(hide);
            _owner            = owner;
            _playerController = playerController;
            
            RefuelButton.onClick.AddListener(OnRefuelClick);
            RefuelButton.OnHoverStart  += OnRefuelHover;
            RefuelButton.OnHoverFinish += HideTooltip;
            
            RepairButton.onClick.AddListener(OnRepairClick);
            RepairButton.OnHoverStart  += OnRepairHover;
            RepairButton.OnHoverFinish += HideTooltip;
            
            TakeoffButton.onClick.AddListener(OnTakeoffClick);
            TakeoffButton.OnHoverStart  += OnTakeoffHover;
            TakeoffButton.OnHoverFinish += HideTooltip;
            
            HideButton.onClick.AddListener(OnHideClick);
            HideButton.OnHoverStart  += OnHideHover;
            HideButton.OnHoverFinish += HideTooltip;
        }

        protected override void DeinitSpecific() {
            _playerController = null;
            
            RefuelButton.onClick.RemoveAllListeners();
            RefuelButton.OnHoverStart  -= OnRefuelHover;
            RefuelButton.OnHoverFinish -= HideTooltip;
            
            RepairButton.onClick.RemoveAllListeners();
            RepairButton.OnHoverStart  -= OnRepairClick;
            RepairButton.OnHoverFinish -= HideTooltip;
            
            TakeoffButton.onClick.RemoveAllListeners();
            TakeoffButton.OnHoverStart  -= OnTakeoffHover;
            TakeoffButton.OnHoverFinish -= HideTooltip;
            
            HideButton.onClick.RemoveAllListeners();
            HideButton.OnHoverStart  -= OnHideHover;
            HideButton.OnHoverFinish -= HideTooltip;
        }

        public override void Show() {
            UpdateFuelPrice();
            UpdateRepairPrice();
        }

        void OnRefuelClick() {
            if ( _fuelAmount <= 0 ) {
                Debug.LogError("Unsupported scenario");
                return;
            }
            _playerController.Money -= _fuelPrice;
            _playerController.Fuel  += _fuelAmount;
            UpdateFuelPrice();
        }

        void OnRefuelHover() {
            TooltipText.text = $"Refuel ({_fuelPrice})";
            ShowTooltip();
        }

        void OnRepairClick() {
            if ( _repairAmount <= 0 ) {
                Debug.LogError("Unsupported scenario");
                return;
            }
            // TODO: repair stuff
        }

        void OnRepairHover() {
            // TODO: set repair text
        }

        void OnTakeoffClick() {
            _owner.Hide();
        }

        void OnTakeoffHover() {
            TooltipText.text = "Take off!";
            ShowTooltip();
        }

        void OnHideClick() {
            Hide();
        }

        void OnHideHover() {
            TooltipText.text = "Go back";
            ShowTooltip();
        }

        void UpdateFuelPrice() {
            var deficiency = PlayerState.MaxFuel - _playerController.Fuel;
            if ( deficiency == 0 ) {
                _fuelPrice  = 0;
                _fuelAmount = 0;
            } else {
                var needMoney = deficiency * 10;
                if ( needMoney <= _playerController.Money ) {
                    _fuelPrice  = needMoney;
                    _fuelAmount = deficiency;
                } else {
                    _fuelPrice  = _playerController.Money - _playerController.Money % 10;
                    _fuelAmount = _fuelPrice / 10;
                }
            }
            RefuelButton.interactable = (_fuelAmount > 0);
        }

        void UpdateRepairPrice() {
            // TODO: get ready repair stuff
            RepairButton.interactable = false;
        }

        void ShowTooltip() {
            Tooltip.Show();
        }

        void HideTooltip() {
            Tooltip.Hide();
        }
    }
}
