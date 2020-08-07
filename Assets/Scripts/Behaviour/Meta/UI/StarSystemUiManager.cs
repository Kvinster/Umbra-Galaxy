using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;

using STP.Behaviour.Starter;

namespace STP.Behaviour.Meta.UI {
    public sealed class StarSystemUiManager : BaseMetaComponent {
        public EnterSystemButton      EnterSystemButton;
        public GameObject             StarSystemScreenRoot;
        public StarSystemTradeScreen  TradeScreen;
        public StarSystemHangarScreen HangarScreen;
        public Button                 ShowTradeScreenButton;
        public Button                 ShowHangarScreenButton;

        readonly List<BaseStarSystemSubScreen> _subScreens = new List<BaseStarSystemSubScreen>();

        MetaUiManager _owner;

        bool _isStarSystemScreenActive;
        public bool IsStarSystemScreenActive {
            get => _isStarSystemScreenActive;
            private set {
                if ( _isStarSystemScreenActive == value ) {
                    return;
                }
                _isStarSystemScreenActive = value;
                StarSystemScreenRoot.SetActive(_isStarSystemScreenActive);
                OnStarSystemScreenActiveChanged?.Invoke(_isStarSystemScreenActive);
            }
        }

        public event Action<bool> OnStarSystemScreenActiveChanged;

        void OnDestroy() {
            TradeScreen.Deinit();
            HangarScreen.Deinit();
        }

        protected override void InitInternal(MetaStarter starter) {
            EnterSystemButton.Init(this, starter.TimeManager, starter.StarSystemsController,
                starter.PlayerController);

            TradeScreen.Init(HideTradeScreen, starter.StarSystemsManager, starter.InventoryItemInfos,
                starter.ProgressController, starter.StarSystemsController, starter.PlayerController);
            HangarScreen.Init(HideHangarScreen, this, starter.PlayerController);
            
            IsStarSystemScreenActive = false;
            StarSystemScreenRoot.SetActive(_isStarSystemScreenActive);

            _subScreens.Add(TradeScreen);
            _subScreens.Add(HangarScreen);

            ShowTradeScreenButton.onClick.AddListener(ShowTradeScreen);
            ShowHangarScreenButton.onClick.AddListener(ShowHangarScreen);
        }

        public void ShowStarSystemScreen() {
            IsStarSystemScreenActive = true;
        }

        public void Hide() {
            foreach ( var subScreen in _subScreens ) {
                subScreen.gameObject.SetActive(false);
            }
            IsStarSystemScreenActive = false;
        }
        
        void ShowTradeScreen() {
            TradeScreen.Show();
            TradeScreen.gameObject.SetActive(true);
        }

        void HideTradeScreen() {
            TradeScreen.gameObject.SetActive(false);
        }

        void ShowHangarScreen() {
            HangarScreen.Show();
            HangarScreen.gameObject.SetActive(true);
        }

        void HideHangarScreen() {
            HangarScreen.gameObject.SetActive(false);
        }
    }
}
