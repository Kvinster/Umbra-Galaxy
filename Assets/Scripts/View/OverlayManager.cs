using System;

using STP.Utils;

namespace STP.View {
    public class OverlayManager : GameBehaviour{
        public MothershipOverlay MotherShipOverlay;
        
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, MotherShipOverlay);

        public void ShowMothershipOverlay(Action<MothershipOverlay> initAction) {
            MotherShipOverlay.gameObject.SetActive(true);
            initAction?.Invoke(MotherShipOverlay);
        }

        public void HideMothershipOverlay() {
            MotherShipOverlay.Deinit();
        }
    }
}