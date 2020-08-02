using UnityEngine;
using UnityEngine.UI;

using STP.Gameplay.Weapon.Common;
using STP.State;

using TMPro;

namespace STP.Behaviour.Meta.UI.InventoryWindow {
    public sealed class WeaponChoiceButton : MonoBehaviour {
        public Image      WeaponIcon;
        public TMP_Text   WeaponTypeText; 
        public GameObject SelectedRoot;
        public Button     Button;

        WeaponType      _weaponType = WeaponType.Unknown;
        InventoryWindow _owner;

        public void Init(WeaponType weaponType, InventoryWindow owner) {
            _weaponType = weaponType;
            _owner      = owner;
            
            // TODO: init WeaponIcon
            WeaponTypeText.text = weaponType.ToString();

            Button.onClick.AddListener(OnClick);

            var ps = PlayerState.Instance;
            ps.OnWeaponChanged += OnWeaponChanged;
            OnWeaponChanged(ps.CurWeaponType);
        }

        public void Deinit() {
            _weaponType = WeaponType.Unknown;
            _owner      = null;

            Button.onClick.RemoveAllListeners();

            PlayerState.Instance.OnWeaponChanged -= OnWeaponChanged;
        }
        
        void OnClick() {
            _owner.TryChooseWeapon(_weaponType);
        }

        void OnWeaponChanged(WeaponType curWeaponType) {
            SelectedRoot.SetActive(curWeaponType == _weaponType);
        }
    }
}
