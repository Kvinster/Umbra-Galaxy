using UnityEngine;

using STP.Utils;

namespace STP.Behaviour.Utils {
	public sealed class DelayedDestroyer : GameComponent {
		public float DestroyTime = 1f;

		Timer _timer;
		bool  _started;

		void Update() {
			if ( !_started ) {
				return;
			}
			if ( _timer.DeltaTick() ) {
				Destroy(gameObject);
			}
		}

		public void StartDestroy() {
			if ( _started ) {
				Debug.LogError("Destroy already started");
				return;
			}
			_started = true;
			_timer   = new Timer();
			_timer.Start(DestroyTime);
		}
	}
}
