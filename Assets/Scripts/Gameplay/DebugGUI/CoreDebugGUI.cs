using STP.Behaviour.Starter;
using System.Collections.Generic;

using STP.View.DebugGUI;

namespace STP.Gameplay.DebugGUI {
    using BaseCoreMenu = BaseMenu<CoreDebugGUI, CoreStarter>;
    
    public class CoreDebugGUI : BaseDebugDrawable<CoreStarter> {
        readonly List<BaseCoreMenu> _currentSubMenuPath = new List<BaseCoreMenu>();
        public BaseCoreMenu ActiveMenu {
            get => _currentSubMenuPath.Count != 0 ? _currentSubMenuPath[_currentSubMenuPath.Count - 1] : null;
            set => _currentSubMenuPath.Add(value);
        }
		
        public CoreDebugGUI(CoreStarter infoHolder) : base(infoHolder) { }

        public override void Draw() {
            if ( ActiveMenu == null ) {
                if ( DebugGuiUtils.Button("show debug UI") ) {
                    ActiveMenu = new CoreDebugMenu(this, InfoHolder);
                }
                return;
            }

            //Default return button for all menus
            if ( DebugGuiUtils.Button("<") ) {
                _currentSubMenuPath.RemoveAt(_currentSubMenuPath.Count-1);
            }
			
            ActiveMenu?.Draw();
        }
    }
}