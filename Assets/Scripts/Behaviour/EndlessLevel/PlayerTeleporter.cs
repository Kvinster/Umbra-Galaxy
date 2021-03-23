using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.EndlessLevel {
	public sealed class PlayerTeleporter : BaseEndlessLevelComponent {
		[NotNull] public Rigidbody2D Player;

		Rect _cameraRect;

		bool IsPlayerOutOfArea => !_cameraRect.Contains(Player.position);
		
		void FixedUpdate() {
			if ( IsPlayerOutOfArea ) {
				TeleportPlayer();
			}
		}
		
		protected override void InitInternal(EndlessLevelStarter starter) {
			var cam     = starter.Camera;
			var height     = cam.orthographicSize * 2;
			var cameraSize = new Vector2(height * cam.aspect, height);
			_cameraRect = new Rect((Vector2)cam.transform.position - cameraSize / 2, cameraSize);
		}

		void TeleportPlayer() {
			var newPos = Player.position;
			var normalizedIntersectPoint = Rect.PointToNormalized(_cameraRect, newPos);
			if ( Mathf.Approximately(normalizedIntersectPoint.y, 1) ) {
				newPos.y = _cameraRect.yMin;
			}
			if ( Mathf.Approximately(normalizedIntersectPoint.y, 0) ) {
				newPos.y = _cameraRect.yMax;
			}
			if ( Mathf.Approximately(normalizedIntersectPoint.x, 1) ) {
				newPos.x = _cameraRect.xMin;
			}
			if ( Mathf.Approximately(normalizedIntersectPoint.x, 0) ) {
				newPos.x = _cameraRect.xMax;
			}
			Player.position = newPos;
		}
	}
}