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
				var leftSide  = new Line(new Vector2(rect.x   , rect.y)   , new Vector2(rect.x   , rect.yMax));
				var rightSide = new Line(new Vector2(rect.xMax, rect.y)   , new Vector2(rect.xMax, rect.yMax));
				var upperSide = new Line(new Vector2(rect.x   , rect.yMax), new Vector2(rect.xMax, rect.yMax));
				var lowerSide = new Line(new Vector2(rect.x   , rect.y)   , new Vector2(rect.xMax, rect.y));

				var randomLine = new Line(targetLocalPos, Vector2.zero);

				foreach ( var line in new[]{leftSide, rightSide, upperSide, lowerSide} ) {
					if ( TryToUpdatePosition(line, randomLine) ) {
						break;
					}
				}

				transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, targetLocalPos));
			}
		}

		bool TryToUpdatePosition(Line one, Line two) {
			var localPoint = GetIntersectPoint(one, two);
			if ( localPoint == null ) {
				return false;
			}
			var pointerScale = transform.lossyScale;
			var newPoint = new Vector2(localPoint.Value.x / pointerScale.x, localPoint.Value.y / pointerScale.y);
			transform.localPosition = newPoint;
			return true;
		}

		Vector2? GetIntersectPoint(Line one, Line two) {
			var x1 = one.Point1.x;
			var x2 = one.Point2.x;
			var x3 = two.Point1.x;
			var x4 = two.Point2.x;
			var y1 = one.Point1.y;
			var y2 = one.Point2.y;
			var y3 = two.Point1.y;
			var y4 = two.Point2.y;

			var divisor = (x1 - x2) * (y4 - y3) - (y1 - y2) * (x4 - x3);
			if ( Mathf.Approximately(divisor, 0) ) {
				return null;
			}

			// https://cpp.mazurok.com/mif-9/
			var numeratorA = (x4-x2)*(y4-y3) - (x4-x3)*(y4-y2);
			var numeratorB = (x1-x2)*(y4-y2) - (x4-x2)*(y1-y2);
			var Ua         = numeratorA / divisor;
			var Ub         = numeratorB / divisor;

			if ( (Ua < 0) || (Ua > 1) || (Ub < 0) || (Ub > 1) ) {
				return null;
			}

			var xDivident    = (x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4);
			var yDivident    = (x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4);
			var pointDivisor = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

			return new Vector2(xDivident / pointDivisor, yDivident / pointDivisor);
		}
	}
}