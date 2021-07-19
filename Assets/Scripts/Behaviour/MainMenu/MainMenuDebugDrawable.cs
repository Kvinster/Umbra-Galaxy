using STP.Behaviour.Starter;
using STP.Events;
using STP.Utils.Events;
using STP.View.DebugGUI;

namespace STP.Behaviour.MainMenu {
	public class MainMenuDebugDrawable : BaseDebugDrawable<MainMenuStarter> {
		public MainMenuDebugDrawable(MainMenuStarter infoHolder) : base(infoHolder) { }

		public override void Draw() {
			if ( DebugGuiUtils.Button("Open all levels") ) {
				EventManager.Fire(new AllLevelButtonsAreAvailable());
			}
		}
	}
}