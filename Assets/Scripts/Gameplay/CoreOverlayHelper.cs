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
        
        public void ShowGameoverOverlay() {
            _overlayManager.HideAllOverlays();
            _overlayManager.ShowGameoverOverlay((x)=>x.Init(_coreManager));
        }
    }
}