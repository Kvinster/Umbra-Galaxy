using Cysharp.Threading.Tasks;
using STP.Utils;
using UnityEngine;

namespace STP.Behaviour.Core {
	public sealed class CameraShake : GameComponent {
		public async UniTask Shake(float duration, float magnitude) {
			var startPos = transform.localPosition;

			var time = 0f;

			while ( time < duration ) {
				var x = Random.Range(-magnitude, magnitude);
				var y = Random.Range(-magnitude, magnitude);

				transform.localPosition = startPos + new Vector3(x, y, 0f);

				time += Time.deltaTime;
				await UniTask.WaitForEndOfFrame();
			}

			transform.localPosition = startPos;
		}
	}
}