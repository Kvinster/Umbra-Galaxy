using UnityEngine;

using STP.Utils;

using Shapes;

namespace STP.Behaviour.Core.UI {
	public class Pointer : GameComponent {
		readonly struct Line {
			public readonly Vector2 Point1;
			public readonly Vector2 Point2;

			public Line(Vector2 point1, Vector2 point2) {
				Point1 = point1;
				Point2 = point2;
			}
		}

		public Polygon ArrowView;

		Transform _target;

		Camera _camera;

		Rect CameraRect {
			get {
				if ( !_camera ) {
					_camera = Camera.main;
				}
				var height     = _camera.orthographicSize * 2;
				var weight     = height * _camera.aspect;
				var screenSize = new Vector2(weight, height);
				return new Rect(-screenSize/2, screenSize);
			}
		}

		public void Init(Transform target) {
			_target  = target;
			_camera = Camera.main;
		}

		public void Deinit() {
			_target = null;
		}

		void Update() {
			if ( !_target ) {
				return;
			}
			var rect            = CameraRect;
			var camPos          =  _camera.transform.position;
			var targetLocalPos  = _target.position - camPos;
			var isInVisibleArea = rect.Contains(targetLocalPos);
			ArrowView.enabled = !isInVisibleArea;
			if ( !isInVisibleArea ) {
				var normalizedPosOnBorder = Rect.PointToNormalized(rect, targetLocalPos);
				var worldPointOnRect      = Rect.NormalizedToPoint(rect, normalizedPosOnBorder);
				transform.localPosition = worldPointOnRect / transform.lossyScale;
				transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, targetLocalPos));
			}
		}
	}
}