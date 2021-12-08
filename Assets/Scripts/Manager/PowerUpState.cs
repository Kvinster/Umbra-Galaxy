using STP.Common;
using STP.Utils;

namespace STP.Manager {
	public sealed class PowerUpState {
		public readonly PowerUpType Type;

		readonly Timer _timer = new Timer();

		public float TimeLeft => _timer.TimeLeft;

		public float Interval => _timer.Interval;

		public PowerUpState(PowerUpType type, float time) {
			Type = type;
			_timer.Start(time);
		}

		public void Reset() {
			_timer.Reset();
		}

		public void AddTime(float time) {
			_timer.Start(_timer.Interval + time, _timer.TimePassed);
		}

		public bool Tick(float deltaTime) {
			return _timer.Tick(deltaTime);
		}
	}
}