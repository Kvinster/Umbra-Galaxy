using UnityEngine.SceneManagement;

using STP.Behaviour.Starter;
using STP.View.DebugGUI;

namespace STP.Gameplay.DebugGUI {
    public class MetaDebugMenu : BaseMenu<MetaDebugGUI, MetaStarter> {
        public MetaDebugMenu(MetaDebugGUI menuHolder, MetaStarter infoHolder) : base(menuHolder, infoHolder) { }

        public override void Draw() {
            if ( DebugGuiUtils.Button("Go to test room") ) {
                SceneManager.LoadScene("TestRoom");
            }
        }
    }
}