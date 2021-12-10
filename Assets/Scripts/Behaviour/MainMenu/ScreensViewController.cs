using System.Collections.Generic;
using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;
using UnityEngine;

namespace STP.Behaviour.MainMenu {
    public class ScreensViewController : BaseMainMenuComponent, IScreenShower {
        [NotNull] public MainScreen        MainScreen;
        [NotNull] public SettingsScreen    SettingsScreen;
        [NotNull] public LeaderboardScreen LeaderboardScreen;
        [NotNull] public QuitScreen        QuitScreen;

        List<IScreen> _screens;

        protected override void InitInternal(MainMenuStarter starter) {
            _screens = new List<IScreen>{ MainScreen, SettingsScreen, LeaderboardScreen, QuitScreen };
            MainScreen.Init(this);
            SettingsScreen.Init(starter);
            LeaderboardScreen.Init(starter);
            QuitScreen.Init(this);

            Show<MainScreen>();
        }

        public void ShowWithoutHiding<T>() where T : class, IScreen {
            var neededScreen = TryGetScreen<T>();
            if ( neededScreen == null ) {
                Debug.LogError($"Can't show screen {typeof(T).Name} - can't find screen");
                return;
            }
            neededScreen.Show();
        }
         
        public void Show<T>() where T : class, IScreen {
            HideAll();
            ShowWithoutHiding<T>();
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
