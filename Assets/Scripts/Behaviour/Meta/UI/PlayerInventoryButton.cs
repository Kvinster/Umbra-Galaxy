using UnityEngine.UI;

using STP.Behaviour.Common;
using STP.Behaviour.Starter;
using STP.Common.Windows;

namespace STP.Behaviour.Meta.UI {
    public sealed class PlayerInventoryButton : BaseMetaComponent {
        public Button Button;
        
        InventoryItemInfos _inventoryItemInfos;

        void Reset() {
            Button = GetComponent<Button>();
        }

        protected override void InitInternal(MetaStarter starter) {
            _inventoryItemInfos = starter.InventoryItemInfos;

            Button.onClick.AddListener(OnClick);
        }

        void OnClick() {
            WindowManager.Instance.Show<InventoryWindow.InventoryWindow>(x => x.Init(_inventoryItemInfos));
        }
    }
}
