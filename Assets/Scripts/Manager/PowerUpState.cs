using STP.Utils;

namespace STP.Manager {
	public sealed class PowerUpState {
		public readonly string Name;

		readonly Timer _timer = new Timer();

		public float TimeLeft => _timer.TimeLeft;

		public PowerUpState(string powerUpName, float time) {
			Name = powerUpName;
			_timer.Start(time);
		}

		public void AddTime(float time) {
			_timer.Start(_timer.Interval + time, _timer.TimePassed);
		}

		public bool Tick(float deltaTime) {
			return _timer.Tick(deltaTime);
		}
	}
}