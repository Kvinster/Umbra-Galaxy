using STP.Utils;

namespace STP.View.DebugGUI {
	public sealed class DebugGuiController : SingleBehaviour<DebugGuiController> {
		IDebugDrawable _currentDrawable;

		public void SetDrawable(IDebugDrawable drawable) {
			_currentDrawable = drawable;
		}

		void OnGUI() {
			_currentDrawable?.Draw();
		}
	}
}