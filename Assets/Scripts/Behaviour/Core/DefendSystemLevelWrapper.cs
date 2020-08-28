using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;
using STP.Common;
using STP.Events;
using STP.Gameplay;
using STP.State;
using STP.State.Meta;
using STP.State.QuestStates;
using STP.Utils.Events;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
    public sealed class DefendSystemLevelWrapper : BaseLevelWrapper {
        [NotNull] public DefendStarGate           StarGate;
        [NotNull] public DefendSystemEnemySpawner EnemySpawner;

        CoreManager           _coreManager;
        LevelController       _levelController;
        StarSystemsController _starSystemsController;
        PlayerController      _playerController;
        QuestsController      _questsController;

        string                 _starSystemId;
        DefendSystemQuestState _questState;

        bool _isActive;

        readonly List<BaseShip> _enemyShips = new List<BaseShip>();

        void OnDestroy() {
            StarGate.OnDestroyed -= OnStarGateDestroyed;
            _coreManager.OnPreGoToMeta -= OnPlayerLeft;
            EnemySpawner.OnEnemySpawned -= OnEnemySpawned;
            foreach ( var enemyShip in _enemyShips ) {
                enemyShip.OnShipDestroyed -= OnEnemyDestroyed;
            }
        }

        public override void Init(CoreStarter starter) {
            _coreManager           = starter.CoreManager;
            _levelController       = starter.LevelController;
            _starSystemsController = starter.StarSystemsController;
            _playerController      = starter.PlayerController;
            _questsController      = starter.QuestsController;

            var testQuestState = new DefendSystemQuestState(1000, _playerController.CurSystemId,
                _playerController.CurSystemId, _playerController.CurSystemId, new RewardInfo());
            _questsController.TryStartQuest(testQuestState);
            _levelController.StartLevel(testQuestState.Id);

            _starSystemId = _playerController.CurSystemId;
            if ( _starSystemsController.GetStarSystemType(_starSystemId) != StarSystemType.Faction ) {
                Debug.LogErrorFormat("Invalid star system '{0}' type '{1}'",
                    _starSystemsController.GetStarSystemName(_starSystemId),
                    _starSystemsController.GetStarSystemType(_starSystemId).ToString());
                return;
            }
            if ( !_starSystemsController.GetFactionSystemActive(_starSystemId) ) {
                Debug.LogErrorFormat("Star system '{0}' is already captured",
                    _starSystemsController.GetStarSystemName(_starSystemId));
                return;
            }

            if ( string.IsNullOrEmpty(_levelController.CurQuestId) ) {
                Debug.LogError("Level quest id is null or empty");
                return;
            }
            var baseQuestState = _questsController.GetQuestState(_levelController.CurQuestId);
            if ( baseQuestState == null ) {
                return;
            }
            if ( (baseQuestState.QuestType != QuestType.DefendSystem) ||
                 !(baseQuestState is DefendSystemQuestState questState) ) {
                Debug.LogErrorFormat("Invalid level quest state QuestType and/or type ('{0}', '{1}')",
                    baseQuestState.QuestType.ToString(), baseQuestState.GetType().Name);
                return;
            }
            if ( questState.Status != QuestStatus.Started ) {
                Debug.LogErrorFormat("Invalid level quest state status '{0}'", questState.Status.ToString());
                return;
            }
            _questState = questState;
            _isActive   = true;

            StarGate.OnDestroyed       += OnStarGateDestroyed;
            _coreManager.OnPreGoToMeta += OnPlayerLeft;
            EnemySpawner.TryStartSpawn();
            EnemySpawner.OnEnemySpawned += OnEnemySpawned;
        }

        void OnEnemySpawned(GameObject enemyGo) {
            if ( !enemyGo ) {
                Debug.LogError("Spawned enemy is null");
                return;
            }
            var enemyShip = enemyGo.GetComponent<EnemyShip>();
            if ( !enemyShip ) {
                Debug.LogErrorFormat("No EnemyShip component on spawned object '{0}'", enemyGo.name);
                return;
            }
            if ( _enemyShips.Contains(enemyShip) ) {
                Debug.LogErrorFormat("EnemyShip '{0}' is already registered", enemyGo.name);
                return;
            }
            enemyShip.OnShipDestroyed += OnEnemyDestroyed;
            _enemyShips.Add(enemyShip);
        }

        void OnEnemyDestroyed(BaseShip baseEnemyShip) {
            if ( !baseEnemyShip ) {
                Debug.LogError("Enemy ship is null");
                return;
            }
            baseEnemyShip.OnShipDestroyed -= OnEnemyDestroyed;
            if ( !_isActive ) {
                return;
            }
            if ( !_enemyShips.Remove(baseEnemyShip) ) {
                Debug.LogErrorFormat("EnemyShip '{0}' wasn't registered", baseEnemyShip.name);
                return;
            }
            if ( (_enemyShips.Count == 0) && !EnemySpawner.IsSpawnActive ) {
                if ( _questsController.TryFinishQuest(_questState) ) {
                    _isActive = false;
                    LevelQuestState = LevelQuestState.Completed;
                    EventManager.Fire(new QuestCompleted());
                } else {
                    Debug.LogError("Can't finish level quest");
                }
            }
        }

        void OnPlayerLeft() {
            if ( !_isActive ) {
                return;
            }
            if ( _questsController.TryFailQuest(_questState) ) {
                _isActive = false;
                LevelQuestState = LevelQuestState.Failed;
                _starSystemsController.SetFactionSystemActive(_starSystemId, false);
            } else {
                Debug.LogError("Can't fail level quest");
            }
        }

        void OnStarGateDestroyed() {
            if ( !_isActive ) {
                Debug.LogError("Destroying star gate when inactive");
                return;
            }
            StarGate.OnDestroyed -= OnStarGateDestroyed;
            if ( _questsController.TryFailQuest(_questState) ) {
                _isActive = false;
                LevelQuestState = LevelQuestState.Failed;
                _starSystemsController.SetFactionSystemActive(_starSystemId, false);
                // TODO: start retreat
            } else {
                Debug.LogError("Can't fail level quest");
            }
        }
    }
}
