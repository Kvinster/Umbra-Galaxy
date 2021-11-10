﻿using UnityEngine;
using System.Collections.Generic;
using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public class ObjectTeleporter : BaseCoreComponent {
		const float BorderMinSideSize = 10f;
		
		[NotNull] [Count(4)] public List<TriggerNotifier> Borders;
		
		Rect _battleArea;

		readonly List<Transform> _checkingObjects = new List<Transform>();
		
		void Update() {
			for ( var i = _checkingObjects.Count - 1; i >= 0; i-- ) {
				var obj = _checkingObjects[i];
				if ( !obj ) {
					_checkingObjects.Remove(obj);
				}
			}
		}

		void OnDestroy() {
			foreach ( var borderControl in Borders ) {
				borderControl.OnTriggerEnter -= OnObjectEnterControlArea;
				borderControl.OnTriggerExit  -= OnObjectLeaveControlArea;
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			var cam        = starter.MainCamera;
			var areaHeight = cam.orthographicSize * 2;
			var areaWidth  = cam.aspect * areaHeight;
			_battleArea = new Rect(new Vector2(-areaWidth/2, -areaHeight/2), new Vector2(areaWidth, areaHeight));
			InitBorders();
		}

		void InitBorders() {
			InitColliderSize(Borders[0], new Vector2(_battleArea.size.x + BorderMinSideSize * 2, BorderMinSideSize), _battleArea.center + Vector2.up    * (_battleArea.size.y + BorderMinSideSize) / 2);
			InitColliderSize(Borders[1], new Vector2(_battleArea.size.x + BorderMinSideSize * 2, BorderMinSideSize), _battleArea.center + Vector2.down  * (_battleArea.size.y + BorderMinSideSize) / 2);
			InitColliderSize(Borders[2], new Vector2(BorderMinSideSize                    , _battleArea.size.y    ), _battleArea.center + Vector2.left  * (_battleArea.size.x + BorderMinSideSize) / 2);
			InitColliderSize(Borders[3], new Vector2(BorderMinSideSize                    , _battleArea.size.y    ), _battleArea.center + Vector2.right * (_battleArea.size.x + BorderMinSideSize) / 2);

			foreach ( var borderControl in Borders ) {
				borderControl.OnTriggerEnter += OnObjectEnterControlArea;
				borderControl.OnTriggerExit  += OnObjectLeaveControlArea;
			}
		}

		void InitColliderSize(TriggerNotifier notifier, Vector2 size, Vector2 centerPos) {
			notifier.transform.position = centerPos;
			var boxCollider = notifier.GetComponent<BoxCollider2D>();
			boxCollider.size = size;
		}

		void Teleport(Transform obj) {
			obj.position = CalculateNewPosition(obj);
		}

		Vector2 CalculateNewPosition(Transform obj) {
			var objectPos                 = (Vector2) obj.position;
			var vectorFromCenterAreaToObj = (objectPos - _battleArea.center);
			Vector2 teleportationVector;
			if ( Mathf.Abs(vectorFromCenterAreaToObj.y) >= _battleArea.height / 2) {
				//on upper or bottom border
				// y > 0 => upper. otherwise - bottom.
				var teleportationDirection = (vectorFromCenterAreaToObj.y > 0) ? Vector2.down : Vector2.up;
				var vectorSize             = 2 * Mathf.Abs(vectorFromCenterAreaToObj.y);
				teleportationVector = teleportationDirection * vectorSize;
			} else {
				//on left or right border
				//x > 0 => right. otherwise - left.
				var teleportationDirection = (vectorFromCenterAreaToObj.x > 0) ? Vector2.left : Vector2.right;
				var vectorSize             = 2 * Mathf.Abs(vectorFromCenterAreaToObj.x);
				teleportationVector = teleportationDirection * vectorSize;
			}
			return objectPos + teleportationVector;
		}

		bool IsNeedToBeTeleported(Transform obj) {
			return !_battleArea.Contains(obj.position);
		}

		void OnObjectEnterControlArea(GameObject go) {
			_checkingObjects.Add(go.transform);
		}
		
		void OnObjectLeaveControlArea(GameObject go) {
			_checkingObjects.Remove(go.transform);
			if ( IsNeedToBeTeleported(go.transform) ) {
				Teleport(go.transform);
			}
		}
	}
}