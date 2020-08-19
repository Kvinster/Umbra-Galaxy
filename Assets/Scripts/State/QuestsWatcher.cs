using UnityEngine;

using STP.Common;
using STP.State.Meta;

namespace STP.State {
    public sealed class QuestsWatcher {
        readonly TimeController   _timeController;
        readonly QuestsController _questsController;
        
        public QuestsWatcher(TimeController timeController, QuestsController questsController) {
            _timeController   = timeController;
            _questsController = questsController;
            
            _timeController.OnCurDayChanged += OnCurDayChanged;
        }

        void OnCurDayChanged(int curDay) {
            foreach ( var questState in _questsController.GetActiveQuestStates() ) {
                if ( (questState.Status == QuestStatus.Started) && (curDay > questState.ExpirationDay) ) {
                    if ( !_questsController.TryFailQuest(questState) ) {
                        Debug.LogErrorFormat("Unsupported scenario — can't fail quest '{0}'", questState);
                    }
                }
            }
        }
    }
}
