using UnityEngine;

namespace STP.Behaviour.Utils {
	// fun script that got imported with Unity Space UI
	public class TiltWindow : MonoBehaviour {
		public Vector2 Range = new Vector2(5f, 3f);

		Quaternion _startRotation;
		Vector2    _rotation = Vector2.zero;

		void Start () {
			_startRotation = transform.localRotation;
		}

		void Update () {
			var pos = Input.mousePosition;

			var halfWidth  = Screen.width * 0.5f;
			var halfHeight = Screen.height * 0.5f;
			var x          = Mathf.Clamp((pos.x - halfWidth) / halfWidth, -1f, 1f);
			var y          = Mathf.Clamp((pos.y - halfHeight) / halfHeight, -1f, 1f);
			_rotation = Vector2.Lerp(_rotation, new Vector2(x, y), Time.deltaTime * 5f);

			transform.localRotation = _startRotation * Quaternion.Euler(-_rotation.y * Range.y, _rotation.x * Range.x, 0f);
		}
	}
}
