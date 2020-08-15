using STP.Behaviour.Core;

namespace STP.Gameplay {
    public class CoreOverlayHelper {
        readonly OverlayManager _overlayManager;
        readonly CoreManager    _coreManager;
        
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