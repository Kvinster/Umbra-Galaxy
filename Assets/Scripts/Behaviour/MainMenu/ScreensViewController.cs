using System.Collections.Generic;
using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;
using UnityEngine;

namespace STP.Behaviour.MainMenu {
    public class ScreensViewController : BaseMainMenuComponent, IScreenShower {
        [NotNull] public MainScreen        MainScreen;
        [NotNull] public SettingsScreen    SettingsScreen;
        [NotNull] public LeaderboardScreen LeaderboardScreen;

        List<IScreen> _screens;

        protected override void InitInternal(MainMenuStarter starter) {
            _screens = new List<IScreen>{ MainScreen, SettingsScreen, LeaderboardScreen };
            MainScreen.Init(this);
            SettingsScreen.Init(starter);
            LeaderboardScreen.Init(starter);

            Show<MainScreen>();
        }

        public void Show<T>() where T : class, IScreen {
            HideAll();
            var neededScreen = TryGetScreen<T>();
            if ( neededScreen == null ) {
                Debug.LogError($"Can't show screen {typeof(T).Name} - can't find screen");
                return;
            }
            neededScreen.Show();
        }

        void HideAll() {
            foreach ( var screen in _screens ) {
                screen.Hide();
            }
        }

        T TryGetScreen<T>() where T : class, IScreen {
            return _screens.Find(x => x is T) as T;
        }
    }
}
