using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.View.DebugGUI;

namespace STP.Gameplay.DebugGUI {
    using BaseMetaMenu = BaseMenu<MetaDebugGUI, MetaStarter>;

    public class MetaDebugGUI : BaseDebugDrawable<MetaStarter> {
        readonly List<BaseMetaMenu> _currentSubMenuPath = new List<BaseMetaMenu>();
        BaseMetaMenu ActiveMenu {
            get => _currentSubMenuPath.Count != 0 ? _currentSubMenuPath[_currentSubMenuPath.Count - 1] : null;
            set => _currentSubMenuPath.Add(value);
        }

        public MetaDebugGUI(MetaStarter infoHolder) : base(infoHolder) { }

        public override void Draw() {
            if ( ActiveMenu == null ) {
                if ( DebugGuiUtils.Button("show debug UI") ) {
                    ActiveMenu = new MetaDebugMenu(this, InfoHolder);
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