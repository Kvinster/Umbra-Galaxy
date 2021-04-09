using UnityEngine;

using STP.Behaviour.Core.Enemy;
using STP.Behaviour.Starter;
using STP.Events;
using STP.Utils;
using STP.Utils.Events;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public class EnemiesTrigger : BaseCoreComponent {
		[NotNull] public TriggerNotifier Notifier;
		[NotNull] public BoxCollider2D   TriggerArea;

		Transform _playerTransform;

		void OnDestroy() {
			Notifier.OnTriggerEnter -= TrySetTarget;
			EventManager.Unsubscribe<PlayerShipChanged>(UpdateTarget);
		}

		protected override void InitInternal(CoreStarter starter) {
			Notifier.OnTriggerEnter += TrySetTarget;
			_playerTransform = starter.Player.transform;
			var cam    = starter.MainCamera;
			var height = cam.orthographicSize * 2;
			TriggerArea.size = new Vector2(cam.aspect * height, height);
			EventManager.Subscribe<PlayerShipChanged>(UpdateTarget);
		}

		void UpdateTarget(PlayerShipChanged e) {
			_playerTransform = e.NewPlayer.transform;
		}

		void TrySetTarget(GameObject obj) {
			var enemyComp = obj.GetComponent<BaseControllableEnemy>();
			if ( !enemyComp ) {
				return;
			}
			enemyComp.SetTarget(_playerTransform);
		}
	}
}