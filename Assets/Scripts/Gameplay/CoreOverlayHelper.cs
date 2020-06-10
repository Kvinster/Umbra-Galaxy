using STP.View;

namespace STP.Gameplay {
    public class CoreOverlayHelper {
        readonly OverlayManager _overlayManager;
        readonly CoreManager    _coreManager;

        bool HasOpenedOverlay => _overlayManager.HasOpenedOverlay();
        
        public CoreOverlayHelper(CoreStarter starter) {
            _overlayManager = starter.OverlayManager;
            _coreManager    = starter.CoreManager;
        }

        public void TryShowMothershipOverlay() {
            if ( HasOpenedOverlay ) {
                return;
            }
            _overlayManager.ShowMothershipOverlay((x)=>x.Init(_coreManager));
        }
        
        public void TryShowGameoverOverlay() {
            if ( HasOpenedOverlay ) {
                return;
            }
            _overlayManager.ShowGameoverOverlay((x)=>x.Init(_coreManager));
        }

        public void HideOverlays() {
            _overlayManager.HideAllOverlays();
        }
    }
}