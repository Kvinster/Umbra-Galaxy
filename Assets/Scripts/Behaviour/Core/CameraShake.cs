using Cysharp.Threading.Tasks;
using STP.Utils;
using UnityEngine;

namespace STP.Behaviour.Core {
	public class CameraShake : GameComponent {
		public async UniTask Shake(float duration, float magnitude) {
			var startPos = transform.position;

			var time = 0f;

			while ( time < duration ) {
				var x = Random.Range(-magnitude, magnitude);
				var y = Random.Range(-magnitude, magnitude);

				transform.localPosition = new Vector3(x, y, startPos.z);
				
				time += Time.deltaTime;
				await UniTask.WaitForEndOfFrame();
			}

			transform.position = startPos;
		} 
	}
}