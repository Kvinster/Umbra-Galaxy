using UnityEngine;

using System.Collections.Generic;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public sealed class EnemyDirector : GameComponent {
		[NotNull] public TriggerNotifier TriggerNotifier;

		List<BaseControllableEnemy> _enemies;
		
		public void Init(List<BaseControllableEnemy> enemies) {
			_enemies = enemies;
			foreach (var enemy in _enemies) {
				enemy.OnDestroyed += OnEnemyDied;
			}
			TriggerNotifier.OnTriggerEnter += OnPlayerEnterZone;
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

		void OnEnemyDied(BaseEnemy e) {
			if ( e is BaseControllableEnemy controllable ) {
				_enemies.Remove(controllable);
				controllable.OnDestroyed -= OnEnemyDied;
			}
		}
	}
}