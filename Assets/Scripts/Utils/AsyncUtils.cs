using System;
using System.Threading;

using Cysharp.Threading.Tasks;

namespace STP.Utils {
	public static class AsyncUtils {
		public static void DelayedAction(Action action, float delay, bool ignoreTimeScale = false,
			PlayerLoopTiming playerLoopTiming = PlayerLoopTiming.Update,
			CancellationToken cancellationToken = default) {
			UniTask.Void(async () => {
				await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale, playerLoopTiming, cancellationToken);
				action?.Invoke();
			});
		}
	}
}
