using UnityEngine;

using STP.Utils;

namespace STP.Behaviour.Utils {
	public sealed class TransformFollower : GameComponent {
		Rect      _areaBorder;
		Transform _target;

		public Vector3   Offset;

		public void Init(Camera camera, Transform target, Rect borders) {
			var camSize = new Vector2(camera.aspect * camera.orthographicSize * 2, camera.orthographicSize * 2);
			_areaBorder = new Rect(borders.min + camSize / 2, borders.size - camSize);
			_target     = target;
		}
		
		void Update() {
			if ( !_target ) {
				return;
			}
			if ( _areaBorder.Contains(_target.position) ) {
				transform.position = _target.position + Offset;
			}
			else {
				var normalizedPos = Rect.PointToNormalized(_areaBorder, _target.position);
				var worldPos      = Rect.NormalizedToPoint(_areaBorder, normalizedPos);
				print($"{worldPos} {normalizedPos} {_target.position}");
				transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
			}
		}

		Vector2 ConvertVector(Vector2 vector2) {
			var abs      = (Mathf.Abs(vector2.x) >= 1) ? Mathf.Abs(vector2.x) : 0;
			var abs2     = (Mathf.Abs(vector2.y) >= 1) ? Mathf.Abs(vector2.y) : 0;
			var maxValue = Mathf.Max(abs, abs2);
			return vector2 / maxValue;
		} 
	}
}
