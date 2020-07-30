using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

using STP.Behaviour.Common;
using STP.Behaviour.Meta.UI.FactionSystemWindow;
using STP.Common.Windows;
using STP.Gameplay.Weapon.Common;
using STP.State;

namespace STP.Behaviour.Meta.UI.InventoryWindow {
    public sealed class InventoryWindow : BaseWindow {
        public PlayerInventoryView      InventoryView;
        public List<WeaponChoiceButton> WeaponChoiceButtons;

        public void Init(InventoryItemInfos inventoryItemInfos) {
            InventoryView.Init(inventoryItemInfos);

            var weaponTypes = ((WeaponType[]) Enum.GetValues(typeof(WeaponType))).ToList();
            weaponTypes.Remove(WeaponType.Unknown);
            if ( weaponTypes.Count > WeaponChoiceButtons.Count ) {
                Debug.LogErrorFormat("Not enough WeaponChoiceButtons: need '{0}', have '{1}'", weaponTypes.Count,
                    WeaponChoiceButtons.Count);
                return;
            }
            var weaponChoiceButtonIndex = 0;
            foreach ( var weaponType in weaponTypes ) {
                var weaponChoiceButton = WeaponChoiceButtons[weaponChoiceButtonIndex++];
                weaponChoiceButton.Init(weaponType, this);
                weaponChoiceButton.gameObject.SetActive(true);
            }
            for ( ; weaponChoiceButtonIndex < WeaponChoiceButtons.Count; ++weaponChoiceButtonIndex ) {
                WeaponChoiceButtons[weaponChoiceButtonIndex].gameObject.SetActive(false);
            }

        }
        
        protected override void Deinit() {
            InventoryView.Deinit();
            foreach ( var weaponChoiceButton in WeaponChoiceButtons ) {
                weaponChoiceButton.Deinit();
            }
        }

        public void TryChooseWeapon(WeaponType weaponType) {
            PlayerState.Instance.CurWeaponType = weaponType;
        }
    }
}
