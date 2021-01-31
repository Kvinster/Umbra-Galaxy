using STP.Core.State;

namespace STP.Core {
	public class SettingsController : BaseStateController {
		readonly GameState     _gameState;
		readonly SettingsState _state;

		public float MasterVolume {
			get => _state.MasterVolume;
			set {
				_state.MasterVolume = value;
				_gameState.Save();
			}
		}

		public SettingsController(GameState gameState) {
			_state     = gameState.SettingsState;
			_gameState = gameState;
		}
	}
}