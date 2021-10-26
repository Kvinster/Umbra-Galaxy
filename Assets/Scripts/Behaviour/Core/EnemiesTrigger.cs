using UnityEngine;

using STP.Behaviour.Core.Enemy;
using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public class EnemiesTrigger : BaseCoreComponent {
		[NotNull] public TriggerNotifier Notifier;
		[NotNull] public BoxCollider2D   TriggerArea;

		Transform _playerTransform;

		void OnDestroy() {
			Notifier.OnTriggerEnter -= TrySetTarget;
			Notifier.OnTriggerExit  -= TryRemoveTarget;
		}

		protected override void InitInternal(CoreStarter starter) {
			Notifier.OnTriggerEnter += TrySetTarget;
			Notifier.OnTriggerExit  += TryRemoveTarget;
			_playerTransform        =  starter.Player.transform;
			var cam    = starter.MainCamera;
			var height = cam.orthographicSize * 2;
			TriggerArea.size = new Vector2(cam.aspect * height, height);
		}

		void TryRemoveTarget(GameObject obj) {
			var enemyComp = obj.GetComponent<BaseEnemy>();
			if ( !enemyComp ) {
				return;
			}
			enemyComp.OnBecomeInvisibleForPlayer();
		}

		void TrySetTarget(GameObject obj) {
			var enemyComp = obj.GetComponent<BaseEnemy>();
			if ( !enemyComp ) {
				return;
			}
			enemyComp.OnBecomeVisibleForPlayer(_playerTransform);
		}
	}
}