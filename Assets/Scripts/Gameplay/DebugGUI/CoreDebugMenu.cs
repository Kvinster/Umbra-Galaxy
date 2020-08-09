using STP.Events;
using STP.Utils.Events;
using STP.View.DebugGUI;

namespace STP.Gameplay.DebugGUI {
    public class CoreDebugMenu : BaseMenu<CoreDebugGUI, CoreStarter> {
        public CoreDebugMenu(CoreDebugGUI menuHolder, CoreStarter infoHolder) : base(menuHolder, infoHolder) { }

        public override void Draw() {
            if ( DebugGuiUtils.Button("Complete quest") ) {
                EventManager.Fire(new QuestCompleted());
            }
            DebugGuiUtils.TextArea($"Level state: {InfoHolder.LevelWrapper.LevelQuestState}", relativeW: 5); 
        }
    }
}