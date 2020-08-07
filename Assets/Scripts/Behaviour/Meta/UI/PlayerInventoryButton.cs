using UnityEngine.UI;

using STP.Behaviour.Common;
using STP.Behaviour.Starter;
using STP.Common.Windows;
using STP.State;

namespace STP.Behaviour.Meta.UI {
    public sealed class PlayerInventoryButton : BaseMetaComponent {
        public Button Button;
        
        InventoryItemInfos _inventoryItemInfos;
        PlayerController   _playerController;

        void Reset() {
            Button = GetComponent<Button>();
        }

        protected override void InitInternal(MetaStarter starter) {
            _inventoryItemInfos = starter.InventoryItemInfos;
            _playerController   = starter.PlayerController;

            Button.onClick.AddListener(OnClick);
        }

        void OnClick() {
            WindowManager.Instance.Show<InventoryWindow.InventoryWindow>(x =>
                x.Init(_inventoryItemInfos, _playerController));
        }
    }
}
