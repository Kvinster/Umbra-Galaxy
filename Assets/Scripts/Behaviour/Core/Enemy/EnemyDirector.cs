﻿using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public sealed class EnemyDirector : BaseCoreComponent {
		[NotNull]
		public TriggerNotifier TriggerNotifier;
		[NotNullOrEmpty]
		public List<BaseEnemy> Enemies;

		Dictionary<BaseEnemy, Vector3> _startPositions;

		Player _player;

		void OnDestroy() {
			TriggerNotifier.OnTriggerEnter -= OnPlayerEnterZone;
			_player.OnPlayerRespawn        -= OnPlayerDied;
		}

		protected override void InitInternal(CoreStarter starter) {
			_player         = starter.Player;
			_startPositions = new Dictionary<BaseEnemy, Vector3>();
			foreach ( var enemy in Enemies ) {
				enemy.OnDestroyed      += OnEnemyDied;
				_startPositions[enemy] =  enemy.transform.position;
			}

			TriggerNotifier.OnTriggerEnter += OnPlayerEnterZone;
			_player.OnPlayerRespawn        += OnPlayerDied;
		}

		void OnPlayerEnterZone(GameObject obj) {
			var player = obj.GetComponent<Player>();
			if ( !player ) {
				return;
			}
			foreach ( var enemy in Enemies ) {
				enemy.SetTarget(player.transform);
			}
		}

		void OnPlayerDied() {
			foreach ( var enemy in Enemies ) {
				enemy.transform.position = _startPositions[enemy];
				enemy.SetTarget(null);
			}
		}

		void OnEnemyDied(BaseEnemy e) {
			if ( !(e is BaseEnemy controllable) ) {
				return;
			}
			Enemies.Remove(controllable);
			_startPositions.Remove(controllable);
			controllable.OnDestroyed -= OnEnemyDied;
		}
	}
}
