using System;
using System.Collections.Generic;

using STP.Behaviour.Common;
using STP.Behaviour.Starter;
using STP.Common.Windows;

namespace STP.Behaviour.Meta.UI {
    public sealed class MetaUiCanvas : BaseMetaComponent {
        public MetaDebugInfoText DebugInfoText;
        public EnterSystemButton EnterSystemButton;
        public InventoryButton   InventoryButton;
        public MetaNewsBlock     NewsBlock;
        public GameOverScreen    GameOverScreen;
        
        StarSystemsManager _starSystemsManager;
        InventoryItemInfos _inventoryItemInfos;
        
        WindowManager WindowManager => WindowManager.Instance;

        readonly List<Type> _ignoreWindowTypes = new List<Type> { typeof(InventoryItemSellWindow) };

        void OnDestroy() {
            WindowManager.OnWindowShown  -= OnWindowShown;
            WindowManager.OnWindowHidden -= OnWindowHidden;
        }

        protected override void InitInternal(MetaStarter starter) {
            _starSystemsManager = starter.StarSystemsManager;
            _inventoryItemInfos = starter.InventoryItemInfos;
            
            DebugInfoText.Init(starter);
            DebugInfoText.gameObject.SetActive(true);

            EnterSystemButton.CommonInit(this, starter.PlayerShip.MovementController, starter.TimeManager,
                starter.StarSystemsManager);
            EnterSystemButton.gameObject.SetActive(true);

            InventoryButton.CommonInit(this, starter.PlayerShip.MovementController, starter.TimeManager,
                starter.StarSystemsManager);
            InventoryButton.gameObject.SetActive(true);

            NewsBlock.Init(starter.TimeManager);
            NewsBlock.gameObject.SetActive(true);

            GameOverScreen.CommonInit(this);
            GameOverScreen.gameObject.SetActive(false);

            WindowManager.OnWindowShown  += OnWindowShown;
            WindowManager.OnWindowHidden += OnWindowHidden;
        }

        public void ShowFactionSystemWindow(string starSystemId) {
            DebugInfoText.gameObject.SetActive(false);
            EnterSystemButton.gameObject.SetActive(false);
            InventoryButton.gameObject.SetActive(false);
            NewsBlock.gameObject.SetActive(false);

            WindowManager.Show<FactionSystemWindow.FactionSystemWindow>(x =>
                x.Init(starSystemId, _starSystemsManager, _inventoryItemInfos));
        }

        public void ShowInventoryWindow() {
            DebugInfoText.gameObject.SetActive(false);
            EnterSystemButton.gameObject.SetActive(false);
            InventoryButton.gameObject.SetActive(false);
            NewsBlock.gameObject.SetActive(false);

            WindowManager.Show<InventoryWindow.InventoryWindow>(x => x.Init(_inventoryItemInfos));
        }

        public void ShowGameOverScreen() {
            GameOverScreen.gameObject.SetActive(true);
        }

        void OnWindowShown(ActiveWindowId windowId) {
            
        }

        void OnWindowHidden(Type windowType) {
            if ( !_ignoreWindowTypes.Contains(windowType) ) {
                DebugInfoText.gameObject.SetActive(true);
                EnterSystemButton.gameObject.SetActive(true);
                InventoryButton.gameObject.SetActive(true);
                NewsBlock.gameObject.SetActive(true);
            }
        }
    }
}
