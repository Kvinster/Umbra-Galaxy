using UnityEngine;
using UnityEngine.UI;

using STP.Gameplay.Weapon.Common;
using STP.State;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.Meta.UI.InventoryWindow {
    public sealed class WeaponChoiceButton : GameBehaviour {
        [NotNull] public Image      WeaponIcon;
        [NotNull] public TMP_Text   WeaponTypeText; 
        [NotNull] public GameObject SelectedRoot;
        [NotNull] public Button     Button;

        WeaponType       _weaponType = WeaponType.Unknown;
        InventoryWindow  _owner;
        PlayerController _playerController;

        public void Init(WeaponType weaponType, InventoryWindow owner, PlayerController playerController) {
            _weaponType       = weaponType;
            _owner            = owner;
            _playerController = playerController;
            
            // TODO: init WeaponIcon
            WeaponTypeText.text = weaponType.ToString();

            Button.onClick.AddListener(OnClick);
            
            _playerController.OnWeaponChanged += OnWeaponChanged;
            OnWeaponChanged(_playerController.CurWeaponType);
        }

        public void Deinit() {
            _weaponType = WeaponType.Unknown;
            _owner      = null;

            Button.onClick.RemoveAllListeners();
            
            if ( _playerController != null ) {
                _playerController.OnWeaponChanged -= OnWeaponChanged;
            }
            _playerController = null;
        }
        
        void OnClick() {
            _owner.TryChooseWeapon(_weaponType);
        }

        void OnWeaponChanged(WeaponType curWeaponType) {
            SelectedRoot.SetActive(curWeaponType == _weaponType);
        }
    }
}
