using UnityEngine;

using STP.Utils;

namespace STP.Behaviour.Utils {
	public sealed class RestrictedTransformFollower : GameComponent {
		public Vector3 Offset;

		Rect      _areaBorder;
		Transform _target;

		public void Init(Camera camera, Transform target, Rect borders) {
			var height  = camera.orthographicSize * 2f;
			var camSize = new Vector2(camera.aspect * height, height);
			_areaBorder = new Rect(borders.min + camSize / 2, borders.size - camSize);
			_target     = target;

			TryUpdatePosition();
		}

		void Update() {
			TryUpdatePosition();
		}

		void TryUpdatePosition() {
			if ( !_target ) {
				return;
			}
			if ( _areaBorder.Contains(_target.position) ) {
				transform.position = _target.position + Offset;
			}
			else {
				var normalizedPos = Rect.PointToNormalized(_areaBorder, _target.position);
				var worldPos      = Rect.NormalizedToPoint(_areaBorder, normalizedPos);
				transform.position = new Vector3(worldPos.x, worldPos.y) + Offset;
			}
		}
	}
}
