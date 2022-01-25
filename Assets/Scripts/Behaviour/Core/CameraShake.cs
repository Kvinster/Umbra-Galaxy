using System.Threading;
using Cysharp.Threading.Tasks;
using STP.Manager;
using STP.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace STP.Behaviour.Core {
	public sealed class CameraShake : GameComponent {
		CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		void OnDestroy() {
			_cancellationTokenSource.Cancel();
		}

		public async UniTask Shake(float duration, float magnitude) {
			var startPos = transform.localPosition;

			var time = 0f;

			while ( time < duration ) {
				if ( _cancellationTokenSource.IsCancellationRequested ) {
					return;
				}
				if ( PauseManager.Instance.IsPaused ) {
					await UniTask.WaitForEndOfFrame();
					continue;
				}
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