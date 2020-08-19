using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

using STP.Common;
using STP.State.Meta;
using STP.State.QuestStates;

using Random = UnityEngine.Random;

namespace STP.State {
    public sealed class QuestsController : BaseStateController {
        readonly QuestsControllerState _state = new QuestsControllerState();

        readonly TimeController         _timeController;
        readonly StarSystemsController  _starSystemsController;
        readonly PlayerController       _playerController;
        readonly DarknessController     _darknessController;
        readonly ShardsActiveController _shardsActiveController;

        readonly QuestsWatcher _questsWatcher;

        public QuestsController(TimeController timeController, StarSystemsController starSystemsController, 
            PlayerController playerController, DarknessController darknessController,
            ShardsActiveController shardsActiveController) {
            _timeController         = timeController;
            _starSystemsController  = starSystemsController;
            _playerController       = playerController;
            _darknessController     = darknessController;
            _shardsActiveController = shardsActiveController;

            // TODO: move to PostLoad if we ever have PostLoad
            _questsWatcher = new QuestsWatcher(_timeController, this);
        }

        public List<BaseQuestState> GetQuestStates() {
            return _state.QuestStates;
        }

        public List<BaseQuestState> GetActiveQuestStates() {
            return _state.QuestStates.Where(x => x.Status == QuestStatus.Started).ToList();
        }

        public BaseQuestState TryPrepareQuest(string originSystemId) {
            var activeQuests = GetActiveQuestStates();
            if ( activeQuests.Count >= 3 ) {
                return null;
            }
            foreach ( var questState in GetActiveQuestStates() ) {
                if ( questState.OriginSystemId == originSystemId ) {
                    return null;
                }
            }
            return TryCreateRandomQuest(originSystemId);
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

        public bool TryFinishQuest(BaseQuestState questState) {
            if ( questState.Status != QuestStatus.Created ) {
                Debug.LogErrorFormat("Invalid quest status '{0}'", questState.Status.ToString());
                return false;
            }
            questState.Status = QuestStatus.Finished;
            return true;
        }

        public bool TryCompleteQuest(BaseQuestState questState) {
            if ( !CanCompleteQuest(questState) ) {
                return false;
            }
            questState.Status = QuestStatus.Completed;
            return true;
        }

        public bool TryFailQuest(BaseQuestState questState) {
            if ( questState.Status != QuestStatus.Started ) {
                Debug.LogErrorFormat("Can't fail quest '{0}' — invalid quest status '{1}'", questState,
                    questState.Status.ToString());
                return false;
            }
            questState.Status = QuestStatus.Failed;
            // TODO: event here?
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

        BaseQuestState TryCreateRandomQuest(string originSystemId) {
            // TODO: proper creation logic, examples below
            // If the origin system is about to be attacked by the Darkness, the quest probably should be to defend this system (tho that might be decided before the
            // CreateRandomQuest is called).
            // If the origin system is a neighbour of a captured system, in all likelihood the quest should be to reclaim the neighbouring system.
            // Otherwise the type of the quest might be decided by random, tho there could be some inclination towards particular types of quests
            // depending on the origin system's faction. For example, the poorer faction's system might ask for GatherResource quests more,
            // while science-y faction would order escorts of its scientists in the shards, etc
            // Quests difficulty should also be taken into account so that the quests aren't too difficult early in the game and not too easy in the endgame.
            var questTypes = ((QuestType[]) Enum.GetValues(typeof(QuestType))).ToList();
            questTypes.Remove(QuestType.Unknown);
            QuestType questType;
            do {
                if ( questTypes.Count == 0 ) {
                    // can't create any type of random quest
                    return null;
                }
                var rand = Random.Range(0, questTypes.Count);
                questType = questTypes[rand];
                questTypes.Remove(questType);
            } while ( !CanCreateRandomQuest(originSystemId, questType) );
            return CreateRandomQuest(originSystemId, questType);
        }

        bool CanCreateRandomQuest(string originSystemId, QuestType questType) {
            switch ( questType ) {
                case QuestType.GatherResource: {
                    return _starSystemsController.GetFactionSystemActive(originSystemId);
                }
                case QuestType.Escort: {
                    var shardSystems   = _starSystemsController.GetActiveShardSystemIds()
                        .Where(x => _shardsActiveController.GetShardActiveDaysRemaining(x) >=
                                    _starSystemsController.GetPath(originSystemId, x).PathLength).ToList();
                    var factionSystems = _starSystemsController.GetFactionSystemIds()
                        .Where(x => !_starSystemsController.GetFactionSystemActive(x)).ToList();
                    return (shardSystems.Count > 0) || (factionSystems.Count > 0);
                }
                case QuestType.ReclaimSystem: {
                    var originFaction  = _starSystemsController.GetFactionSystemFaction(originSystemId);
                    var factionSystems = _starSystemsController.GetFactionSystemIds()
                        .Where(x => !_starSystemsController.GetFactionSystemActive(x) &&
                                    (_starSystemsController.GetFactionSystemFaction(x) == originFaction)).ToList();
                    return (factionSystems.Count > 0);
                }
                case QuestType.DefendSystem: {
                    var maxDistance = DarknessController.DarknessHitTime -
                                      (_timeController.CurDay % DarknessController.DarknessHitTime);
                    var threatenedSystems = _starSystemsController.GetFactionSystemIds()
                        .Where(x => {
                            if ( x == originSystemId ) {
                                return false;
                            }
                            if ( !_starSystemsController.GetFactionSystemActive(x) ||
                                 !_darknessController.IsFactionSystemNextToHit(x) ) {
                                return false;
                            } 
                            var dist = _starSystemsController.GetPath(originSystemId, x).PathLength;
                            return (dist <= maxDistance);
                        })
                        .ToList();
                    return (threatenedSystems.Count > 0);
                }
                case QuestType.Delivery: {
                    if ( !_playerController.Inventory.HasEmptySpace() ) {
                        return false;
                    }
                    var factionSystems = _starSystemsController.GetFactionSystemIds()
                        .Where(x => _starSystemsController.GetFactionSystemActive(x) && (x != originSystemId)).ToList();
                    return (factionSystems.Count > 0);
                }
                default: {
                    Debug.LogErrorFormat("Unsupported quest type '{0}'", questType.ToString());
                    return false;
                }
            }
        }

        BaseQuestState CreateRandomQuest(string originSystemId, QuestType questType) {
            // TODO: when creating Escort level check that no ReclaimSystem quest for the same dest system is active and vice versa
            var reward = new RewardInfo {
                Money = 100
            };
            switch ( questType ) {
                case QuestType.GatherResource: {
                    var starSystems = _starSystemsController.GetFactionSystemIds().Where(x =>
                        _starSystemsController.GetFactionSystemActive(x) && (x != originSystemId)).ToList();
                    Debug.Assert(starSystems.Count > 0);
                    var destSystemId  = originSystemId;
                    var expirationDay = _timeController.CurDay + 10;
                    var questState = new GatherResourcesQuestState(ItemNames.Mineral, 10, expirationDay, originSystemId,
                        destSystemId, originSystemId, reward);
                    return questState;
                }
                case QuestType.Escort: {
                    var shardSystems   = _starSystemsController.GetActiveShardSystemIds()
                        .Where(x => _shardsActiveController.GetShardActiveDaysRemaining(x) >=
                                    _starSystemsController.GetPath(originSystemId, x).PathLength).ToList();
                    var factionSystems = _starSystemsController.GetFactionSystemIds()
                        .Where(x => !_starSystemsController.GetFactionSystemActive(x)).ToList();
                    Debug.Assert((shardSystems.Count > 0) || (factionSystems.Count > 0));

                    var closestId   = string.Empty;
                    var closestDist = int.MaxValue;

                    void CheckList(List<string> list) {
                        foreach ( var systemId in list ) {
                            var dist = _starSystemsController.GetPath(originSystemId, systemId).PathLength;
                            if ( string.IsNullOrEmpty(closestId) || (dist < closestDist) ) {
                                closestId   = systemId;
                                closestDist = dist;
                            }
                        }
                    }

                    CheckList(shardSystems);
                    CheckList(factionSystems);

                    return new EscortQuestState(
                        _timeController.CurDay + closestDist + 5, originSystemId, closestId, originSystemId, reward);
                }
                case QuestType.ReclaimSystem: {
                    var originFaction  = _starSystemsController.GetFactionSystemFaction(originSystemId);
                    var factionSystems = _starSystemsController.GetFactionSystemIds()
                        .Where(x => !_starSystemsController.GetFactionSystemActive(x) &&
                                    (_starSystemsController.GetFactionSystemFaction(x) == originFaction)).ToList();
                    var closestId   = string.Empty;
                    var closestDist = int.MaxValue;
                    foreach ( var factionSystem in factionSystems ) {
                        var dist = _starSystemsController.GetPath(factionSystem, originSystemId).PathLength;
                        if ( string.IsNullOrEmpty(closestId) || (closestDist > dist) ) {
                            closestId   = factionSystem;
                            closestDist = dist;
                        }
                    }
                    return new ReclaimSystemQuestState(_timeController.CurDay + closestDist + 5, originSystemId,
                        closestId, originSystemId, reward);
                }
                case QuestType.DefendSystem: {
                    var maxDistance = DarknessController.DarknessHitTime -
                                      (_timeController.CurDay % DarknessController.DarknessHitTime);
                    var threatenedSystems = _starSystemsController.GetFactionSystemIds()
                        .Where(x => {
                            if ( x == originSystemId ) {
                                return false;
                            }
                            if ( !_starSystemsController.GetFactionSystemActive(x) ||
                                 !_darknessController.IsFactionSystemNextToHit(x) ) {
                                return false;
                            } 
                            var dist = _starSystemsController.GetPath(originSystemId, x).PathLength;
                            return (dist <= maxDistance);
                        })
                        .ToList();
                    Debug.Assert(threatenedSystems.Count > 0);
                    
                    var closestId   = string.Empty;
                    var closestDist = int.MaxValue;
                    foreach ( var factionSystem in threatenedSystems ) {
                        var dist = _starSystemsController.GetPath(factionSystem, originSystemId).PathLength;
                        if ( string.IsNullOrEmpty(closestId) || (closestDist > dist) ) {
                            closestId   = factionSystem;
                            closestDist = dist;
                        }
                    }
                    return new DefendSystemQuestState(_timeController.CurDay + maxDistance, originSystemId, closestId,
                        originSystemId, reward);
                }
                case QuestType.Delivery: {
                    var factionSystems = _starSystemsController.GetFactionSystemIds()
                        .Where(x => _starSystemsController.GetFactionSystemActive(x) && (x != originSystemId)).ToList();
                    var destSystemId = factionSystems[Random.Range(0, factionSystems.Count)];
                    var dist         = _starSystemsController.GetPath(originSystemId, destSystemId).PathLength;
                    var itemName     = ItemNames.UniqueItems[Random.Range(0, ItemNames.UniqueItems.Count)];
                    return new DeliveryQuestState(itemName, _timeController.CurDay + dist + 5, originSystemId,
                        destSystemId, destSystemId, reward);
                }
                default: {
                    Debug.LogErrorFormat("Unsupported quest type '{0}'", questType.ToString());
                    return null;
                }
            }
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
                case QuestType.Escort:
                case QuestType.ReclaimSystem:
                case QuestType.DefendSystem: {
                    return (baseQuestState.Status == QuestStatus.Finished);
                }
                case QuestType.Delivery when baseQuestState is DeliveryQuestState questState: {
                    return (_playerController.Inventory.GetItemAmount(questState.ResourceType) >= 1);
                }
                default: {
                    Debug.LogErrorFormat("Unsupported quest type '{0}'", baseQuestState.QuestType.ToString());
                    return false;
                }
            }
        }
    }
}
