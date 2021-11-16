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
			var cam        = starter.MainCamera;
			var areaHeight = cam.orthographicSize * 2;
			var areaWidth  = cam.aspect * areaHeight;
			_battleArea = new Rect(new Vector2(-areaWidth / 2, -areaHeight / 2), new Vector2(areaWidth, areaHeight));
			
			BoxCollider2D.size = _battleArea.size;

			BattleArea.OnTriggerExit        += OnBattleAreaExit;
			SpawnControlArea.OnTriggerEnter += OnSpawn;
		}

		void OnDestroy() {
			BattleArea.OnTriggerExit        -= OnBattleAreaExit;
			SpawnControlArea.OnTriggerEnter -= OnSpawn;
		}

		void Teleport(Transform obj) {
			var newPos = CalculateNewPosition(obj);
			obj.position = newPos;
		}

		void OnBattleAreaExit(GameObject other) {
			var teleportingObjectTransform = other.gameObject.transform;
			if ( IsNeedToBeTeleported(teleportingObjectTransform) ) {
				Teleport(teleportingObjectTransform);
			}
		}

		void OnSpawn(GameObject other) {
			if ( IsNeedToBeTeleported(other.transform) ) {
				Teleport(other.transform);
			}
		}
		
		Vector2 CalculateNewPosition(Transform obj) {
			var objectPos                 = (Vector2) obj.position;
			var vectorFromCenterAreaToObj = (objectPos - _battleArea.center);


			var rect = Rect.zero;
			var sr   = obj.GetComponentInChildren<SpriteRenderer>();
			if ( sr ) {
				rect = sr.sprite.rect;
			}
			var shapesRenderer = obj.GetComponentInChildren<ShapeRenderer>();
			if ( shapesRenderer ) {
				var bounds = shapesRenderer.Mesh.bounds;
				rect = new Rect(bounds.min, bounds.max - bounds.min);
			}
			if ( rect == Rect.zero ) {
				Debug.LogError($"Can't get object rect => no teleporting for {obj.gameObject.name}");
				return objectPos;
			}

			if ( Mathf.Abs(vectorFromCenterAreaToObj.y) >= _battleArea.height / 2) {
				// y > 0 => upper. otherwise - bottom.
				var newY = ( vectorFromCenterAreaToObj.y > 0 ) 
					? _battleArea.yMin - rect.height / 2f + 2f
					: _battleArea.yMax + rect.height / 2f - 2f;
				return new Vector2(objectPos.x, newY);
			} else {
				// y > 0 => upper. otherwise - bottom.
				var newX = ( vectorFromCenterAreaToObj.x > 0 ) 
					? _battleArea.xMin - rect.width / 2f
					: _battleArea.xMax + rect.width / 2f;
				return new Vector2(newX, objectPos.y);
			}
		}

		bool IsNeedToBeTeleported(Transform obj) {
			return !_battleArea.Contains(obj.position);
		}
	}
}
