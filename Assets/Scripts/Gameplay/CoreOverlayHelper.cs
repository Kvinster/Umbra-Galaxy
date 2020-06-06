using STP.View;

namespace STP.Gameplay {
    public class CoreOverlayHelper {
        readonly OverlayManager _overlayManager;
        readonly CoreManager    _coreManager;

        public CoreOverlayHelper(CoreStarter starter) {
            _overlayManager = starter.OverlayManager;
            _coreManager    = starter.CoreManager;
        }

        public void ShowMothershipOverlay() {
            _overlayManager.ShowMothershipOverlay((x)=>x.Init(_coreManager));
        }

        public void HideMothershipOverlay() {
            _overlayManager.HideMothershipOverlay();
        }
    }
}