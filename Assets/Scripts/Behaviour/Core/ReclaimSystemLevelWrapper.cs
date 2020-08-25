using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;
using STP.Common;
using STP.Events;
using STP.State;
using STP.State.Meta;
using STP.State.QuestStates;
using STP.Utils.Events;

namespace STP.Behaviour.Core {
    public sealed class ReclaimSystemLevelWrapper : BaseLevelWrapper {
        public List<BaseShip> InitEnemiesToDestroy = new List<BaseShip>();

        LevelController       _levelController;
        StarSystemsController _starSystemsController;
        PlayerController      _playerController;
        QuestsController      _questsController;

        readonly List<BaseShip> _enemiesToDestroy = new List<BaseShip>();

        string                  _starSystemId;
        ReclaimSystemQuestState _questState;

        bool _isActive;

        public override void Init(CoreStarter starter) {
            _levelController       = starter.LevelController;
            _starSystemsController = starter.StarSystemsController;
            _playerController      = starter.PlayerController;
            _questsController      = starter.QuestsController;

            _starSystemId = _playerController.CurSystemId;
            if ( _starSystemsController.GetStarSystemType(_starSystemId) != StarSystemType.Faction ) {
                Debug.LogErrorFormat("Invalid star system '{0}' type '{1}'",
                    _starSystemsController.GetStarSystemName(_starSystemId),
                    _starSystemsController.GetStarSystemType(_starSystemId).ToString());
                return;
            }
            if ( _starSystemsController.GetFactionSystemActive(_starSystemId) ) {
                Debug.LogErrorFormat("Star system '{0}' is not captured",
                    _starSystemsController.GetStarSystemName(_starSystemId));
                return;
            }

            if ( string.IsNullOrEmpty(_levelController.CurQuestId) ) {
                _questState = null;
            } else {
                var baseQuestState = _questsController.GetQuestState(_levelController.CurQuestId);
                if ( baseQuestState == null ) {
                    return;
                }
                if ( (baseQuestState.QuestType != QuestType.ReclaimSystem) ||
                     !(baseQuestState is ReclaimSystemQuestState questState) ) {
                    Debug.LogErrorFormat("Invalid level quest state QuestType and/or type ('{0}', '{1}')",
                        baseQuestState.QuestType.ToString(), baseQuestState.GetType().Name);
                    return;
                }
                if ( questState.Status != QuestStatus.Started ) {
                    Debug.LogErrorFormat("Invalid level quest state status '{0}'", questState.Status.ToString());
                    return;
                }
                _questState = questState;
            }
            _isActive   = true;

            foreach ( var initShip in InitEnemiesToDestroy ) {
                RegisterEnemyShip(initShip);
            }
            _enemiesToDestroy.TrimExcess();
            InitEnemiesToDestroy = null;
        }

        public void RegisterEnemyShip(BaseShip ship) {
            if ( !ship ) {
                Debug.LogError("Ship is null");
                return;
            }
            if ( !_isActive ) {
                Debug.LogWarningFormat("Can't register ship '{0}': wrapper is inactive", ship);
                return;
            }
            if ( _enemiesToDestroy.Contains(ship) ) {
                Debug.LogErrorFormat("Ship '{0}' is already registered", ship);
                return;
            }
            if ( ship.CurHp <= 0f ) {
                Debug.LogErrorFormat("Ship '{0}' is already dead", ship);
                return;
            }
            _enemiesToDestroy.Add(ship);
            ship.OnShipDestroyed += OnRegisteredShipDestroyed;
        }

        void OnRegisteredShipDestroyed(BaseShip ship) {
            if ( !_isActive ) {
                Debug.LogErrorFormat("Destroying registered ship '{0}' when inactive", ship);
                return;
            }
            if ( !ship ) {
                Debug.LogError("Ship is null");
                return;
            }
            if ( !_enemiesToDestroy.Remove(ship) ) {
                Debug.LogErrorFormat("Ship '{0}' isn't registered", ship);
                return;
            }
            ship.OnShipDestroyed -= OnRegisteredShipDestroyed;
            if ( _enemiesToDestroy.Count == 0 ) {
                if ( (_questState == null) || _questsController.TryFinishQuest(_questState) ) {
                    _isActive = false;
                    // TODO: move event fire to base class?
                    EventManager.Fire(new QuestCompleted());
                    LevelQuestState = LevelQuestState.Completed;
                    _starSystemsController.SetFactionSystemActive(_starSystemId, true);
                } else {
                    Debug.LogError("Can't finish level quest");
                }
            }
        }
    }
}
