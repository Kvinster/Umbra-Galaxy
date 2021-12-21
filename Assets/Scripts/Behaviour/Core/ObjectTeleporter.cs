using System;
using PlayFab.Internal;
using Shapes;
using UnityEngine;
using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public class ObjectTeleporter : BaseCoreComponent {
		[Header("battle area")]
		[NotNull] public TriggerNotifier BattleArea;
		[NotNull] public BoxCollider2D   BoxCollider2D;
		[Header("fallback area")]
		[NotNull] public TriggerNotifier SpawnControlArea;

		Rect _battleArea;

		protected override void InitInternal(CoreStarter starter) {
			_battleArea = starter.AreaRect;

			BoxCollider2D.size = _battleArea.size;

			BattleArea.OnTriggerExit        += OnBattleAreaExit;
			SpawnControlArea.OnTriggerEnter += OnSpawn;
		}

		void OnDestroy() {
			BattleArea.OnTriggerExit        -= OnBattleAreaExit;
			SpawnControlArea.OnTriggerEnter -= OnSpawn;
		}

		void TryTeleport(Transform obj) {
			var teleportationComponent = obj.GetComponentInChildren<TeleportationColliderController>();
			if ( !_battleArea.Contains(obj.parent.position) && teleportationComponent.KillOnTeleportation ) {
				Destroy(obj.parent.gameObject);
				return;
			}

			var parent                 = obj.parent;
			var newPos                 = CalculateNewPosition(parent, teleportationComponent);
			parent.position = newPos;
		}

		void OnBattleAreaExit(GameObject other) {
			var teleportingObjectTransform = other.gameObject.transform;
			TryTeleport(teleportingObjectTransform);
		}

		void OnSpawn(GameObject other) {
			TryTeleport(other.transform);
		}

		Vector2 CalculateNewPosition(Transform obj, TeleportationColliderController teleportationColliderController) {
			var objectPos                 = (Vector2) obj.position;
			var vectorFromCenterAreaToObj = (objectPos - _battleArea.center);

			var rect = teleportationColliderController.Rect;

			var res = objectPos;

			if ( Mathf.Abs(vectorFromCenterAreaToObj.y) >= _battleArea.height / 2 + rect.height / 2) {
				// y > 0 => upper. otherwise - bottom.
				var newY = ( vectorFromCenterAreaToObj.y > 0 )
					? _battleArea.yMin - rect.height / 2f + 2f
					: _battleArea.yMax + rect.height / 2f - 2f;
				res.y = newY;
			}
			if ( Mathf.Abs(vectorFromCenterAreaToObj.x) >= _battleArea.width / 2 + rect.width / 2) {
				// x > 0 => left. otherwise - right.
				var newX = ( vectorFromCenterAreaToObj.x > 0 )
					? _battleArea.xMin - rect.width / 2f + 2f
					: _battleArea.xMax + rect.width / 2f - 2f;
				res.x = newX;
			}
			return res;
		}

		bool IsNeedToBeTeleported(Transform obj) {
			return !_battleArea.Contains(obj.position);
		}
	}
}