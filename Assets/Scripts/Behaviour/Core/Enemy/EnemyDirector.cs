using UnityEngine;

using System.Collections.Generic;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public sealed class EnemyDirector : GameComponent {
		[NotNull] public TriggerNotifier TriggerNotifier;

		List<BaseControllableEnemy> _enemies;

		Dictionary<BaseControllableEnemy, Vector3> _startPositions;

		Player _player;
		
		void OnDestroy() {
			TriggerNotifier.OnTriggerEnter -= OnPlayerEnterZone;
			_player.OnPlayerRespawn        -= OnPlayerDied;
		}

		public void Init(Player player, List<BaseControllableEnemy> enemies) {
			_enemies = enemies;
			_player  = player;
			_startPositions = new Dictionary<BaseControllableEnemy, Vector3>();
			foreach (var enemy in _enemies) {
				enemy.OnDestroyed      += OnEnemyDied;
				_startPositions[enemy] =  enemy.transform.position;
			}
			
			TriggerNotifier.OnTriggerEnter += OnPlayerEnterZone;
			_player.OnPlayerRespawn        += OnPlayerDied;
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
			if ( e is BaseControllableEnemy controllable ) {
				_enemies.Remove(controllable);
				_startPositions.Remove(controllable);
				controllable.OnDestroyed -= OnEnemyDied;
			}
		}
	}
}