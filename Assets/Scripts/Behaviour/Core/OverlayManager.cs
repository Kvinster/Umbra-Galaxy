using STP.Utils;
using System;
using System.Collections.Generic;

namespace STP.Behaviour.Core {
    public class OverlayManager : GameBehaviour {
        public GameoverOverlay   GameoverOverlay;
        
        List<IOverlay> _allOverlays = new List<IOverlay>();
        
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, GameoverOverlay);

        void Start() {
            _allOverlays = new List<IOverlay>{GameoverOverlay};
            HideAllOverlays();
        }
        
        public void ShowGameoverOverlay(Action<GameoverOverlay> initAction) {
            HideAllOverlays();
            initAction?.Invoke(GameoverOverlay);
        }

        public void HideAllOverlays() {
            foreach ( var overlay in _allOverlays ) {
                overlay.Deinit();
            }
        }

        public bool HasOpenedOverlay() {
            foreach ( var overlay in _allOverlays ) {
                if ( overlay.Active ) {
                    return true;
                }
            }
            return false;
        }
    }
}