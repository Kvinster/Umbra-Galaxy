using UnityEngine;

using STP.Common;

namespace STP.State {
    public sealed class LevelController : BaseStateController {
        readonly LevelControllerState _state = new LevelControllerState();

        readonly QuestsController _questsController;

        public string CurQuestId => _state.CurQuestId;

        public bool IsLevelActive => !string.IsNullOrEmpty(CurQuestId);

        public LevelController(QuestsController questsController) {
            _questsController = questsController;
        }

        public void StartLevel(string questId) {
            if ( IsLevelActive ) {
                Debug.LogErrorFormat("Can't start level with quest id '{0}': level with quest is '{1}' is still active",
                    questId, CurQuestId);
                return;
            }
            var questState = _questsController.GetQuestState(questId);
            if ( questState == null ) {
                return;
            }
            if ( questState.Status != QuestStatus.Started ) {
                Debug.LogErrorFormat("Can't start level with quest id '{0}': invalid quest status '{1}'", questId,
                    questState.Status.ToString());
                return;
            }
            _state.CurQuestId = questId;
        }

        public void FinishLevel() {
            if ( !IsLevelActive ) {
                Debug.LogError("Can't finish level: none active");
                return;
            }
            _state.CurQuestId = string.Empty;
        }
    }
}
