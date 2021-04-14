using UnityEngine;

using System.Collections.Generic;
using STP.Events;
using STP.Utils;
using STP.Utils.Events;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public sealed class EnemyDirector : GameComponent {
		[NotNull] public TriggerNotifier TriggerNotifier;

		List<BaseEnemy> _enemies;

		Dictionary<BaseEnemy, Vector3> _startPositions;

		Player _player;
		
		void OnDestroy() {
			TriggerNotifier.OnTriggerEnter -= OnPlayerEnterZone;
			_player.OnPlayerRespawn -= OnPlayerDied;
			EventManager.Unsubscribe<PlayerShipChanged>(UpdateTarget);
		}

		public void Init(Player player, List<BaseEnemy> enemies) {
			_enemies = enemies;
			_player  = player;
			_startPositions = new Dictionary<BaseEnemy, Vector3>();
			foreach (var enemy in _enemies) {
				enemy.OnDestroyed      += OnEnemyDied;
				_startPositions[enemy] =  enemy.transform.position;
			}
			
			TriggerNotifier.OnTriggerEnter += OnPlayerEnterZone;
			_player.OnPlayerRespawn        += OnPlayerDied;
			EventManager.Subscribe<PlayerShipChanged>(UpdateTarget);
		}

		void OnPlayerEnterZone(GameObject obj) {
			var player = obj.GetComponent<Player>();
			if (!player) {
				return;
			}
			foreach (var enemy in _enemies) {
				enemy.SetTarget(player.transform);
			}
		}

		void OnPlayerDied() {
			foreach ( var enemy in _enemies ) {
				enemy.transform.position = _startPositions[enemy];
				enemy.SetTarget(null);
			}
		}

		void OnEnemyDied(BaseEnemy e) {
			if ( !(e is BaseEnemy controllable) ) {
				return;
			}
			_enemies.Remove(controllable);
			_startPositions.Remove(controllable);
			controllable.OnDestroyed -= OnEnemyDied;
		}
		
		void UpdateTarget(PlayerShipChanged e) {
			_player.OnPlayerRespawn -= OnPlayerDied;
			_player                 =  e.NewPlayer;
			_player.OnPlayerRespawn += OnPlayerDied;
		}
	}
}