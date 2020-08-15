using UnityEngine;

using System.Collections.Generic;
using System.Linq;

using STP.Common;
using STP.State.Meta;
using STP.State.QuestStates;

namespace STP.State {
    public sealed class QuestsController : BaseStateController {
        readonly QuestsControllerState _state = new QuestsControllerState();

        readonly TimeController        _timeController;
        readonly StarSystemsController _starSystemsController;
        readonly PlayerController      _playerController;

        public QuestsController(TimeController timeController, StarSystemsController starSystemsController, 
            PlayerController playerController) {
            _timeController        = timeController;
            _starSystemsController = starSystemsController;
            _playerController      = playerController;

            _timeController.OnCurDayChanged += OnCurDayChanged;
        }

        public List<BaseQuestState> GetQuestStates() {
            return _state.QuestStates;
        }

        public BaseQuestState TryPrepareQuest(string originSystemId) {
            if ( _state.QuestStates.Count >= 3 ) {
                return null;
            }
            var starSystems = _starSystemsController.GetFactionSystemIds().Where(x =>
                _starSystemsController.GetFactionSystemActive(x) && (x != originSystemId)).ToList();
            if ( starSystems.Count == 0 ) {
                return null;
            }
            var destSystemId  = originSystemId;
            var expirationDay = _timeController.CurDay + 10;
            var reward = new RewardInfo {
                Money = 100
            };
            var questState = new GatherResourcesQuestState(ItemNames.Mineral, 10, expirationDay, destSystemId,
                originSystemId, reward);
            return questState;
        }

        public bool TryStartQuest(BaseQuestState questState) {
            if ( questState.Status != QuestStatus.Created ) {
                Debug.LogErrorFormat("Invalid quest status '{0}'", questState.Status.ToString());
                return false;
            }
            if ( _state.QuestStates.Contains(questState) ) {
                Debug.LogError("Quest already active");
                return false;
            }
            questState.Status = QuestStatus.Started;
            _state.QuestStates.Add(questState);
            return true;
        }

        public bool TryCompleteQuest(BaseQuestState questState) {
            if ( !CanCompleteQuest(questState) ) {
                return false;
            }
            questState.Status = QuestStatus.Completed;
            return true;
        } 

        public bool HasReadyToCompleteQuest() {
            foreach ( var questState in _state.QuestStates ) {
                if ( CanCompleteQuest(questState) ) {
                    return true;
                }
            }
            return false;
        }

        public BaseQuestState GetReadyToCompleteQuest() {
            foreach ( var questState in _state.QuestStates ) {
                if ( CanCompleteQuest(questState) ) {
                    return questState;
                }
            }
            Debug.LogError("No ready to complete quests");
            return null;
        }

        bool CanCompleteQuest(BaseQuestState baseQuestState) {
            if ( baseQuestState.Status != QuestStatus.Started ) {
                return false;
            }
            if ( _playerController.CurSystemId != baseQuestState.RewardSystemId ) {
                return false;
            }
            if ( _timeController.CurDay > baseQuestState.ExpirationDay ) {
                return false;
            }
            switch ( baseQuestState.QuestType ) {
                case QuestType.GatherResource when baseQuestState is GatherResourcesQuestState questState: {
                    return (_playerController.Inventory.GetItemAmount(questState.ResourceType) >=
                            questState.ResourceAmount);
                }
                default: {
                    Debug.LogErrorFormat("Unsupported quest type '{0}'", baseQuestState.QuestType.ToString());
                    return false;
                }
            }
        }

        void OnCurDayChanged(int curDay) {
            foreach ( var questState in _state.QuestStates ) {
                if ( questState.Status != QuestStatus.Started ) {
                    continue;
                }
                if ( questState.ExpirationDay <= curDay ) {
                    questState.Status = QuestStatus.Failed;
                    // TODO: event here?
                }
            }
        }
    }
}
