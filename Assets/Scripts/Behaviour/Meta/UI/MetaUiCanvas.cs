using STP.Behaviour.Starter; 

namespace STP.Behaviour.Meta.UI {
    public sealed class MetaUiCanvas : BaseMetaComponent {
        public MetaDebugInfoText                       DebugInfoText;
        public EnterSystemButton                       EnterSystemButton;
        public FactionSystemWindow.FactionSystemWindow FactionSystemWindow;
        public InventoryItemSellWindow                 InventoryItemSellWindow;
        
        protected override void InitInternal(MetaStarter starter) {
            DebugInfoText.Init(starter);
            DebugInfoText.gameObject.SetActive(true);

            EnterSystemButton.CommonInit(this, starter.PlayerShip.MovementController, starter.TimeManager,
                starter.StarSystemsManager);
            EnterSystemButton.gameObject.SetActive(true);

            FactionSystemWindow.CommonInit(this, starter);
            FactionSystemWindow.gameObject.SetActive(false);

            InventoryItemSellWindow.CommonInit(this, starter.InventoryItemInfos);
            InventoryItemSellWindow.gameObject.SetActive(false);
        }

        public void ShowFactionSystemWindow(string starSystemId) {
            DebugInfoText.gameObject.SetActive(false);
            EnterSystemButton.gameObject.SetActive(false);
            FactionSystemWindow.gameObject.SetActive(true);

            FactionSystemWindow.Show(starSystemId);
        }

        public void HideFactionSystemWindow() {
            DebugInfoText.gameObject.SetActive(true);
            EnterSystemButton.gameObject.SetActive(true);
            FactionSystemWindow.gameObject.SetActive(false);
        }

        public void ShowInventoryItemSellWindow(string itemName, string starSystemId) {
            InventoryItemSellWindow.Show(itemName, starSystemId);
            InventoryItemSellWindow.gameObject.SetActive(true);
        }

        public void HideInventoryItemSellWindow() {
            InventoryItemSellWindow.gameObject.SetActive(false);
        }
    }
}
