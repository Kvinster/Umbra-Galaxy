using UnityEngine;
using UnityEngine.Audio;

using STP.Core.State;

namespace STP.Core {
	public class SettingsController : BaseStateController {
		public const string MasterVolumeId = "masterVol";

		readonly GameState     _gameState;
		readonly SettingsState _state;

		readonly AudioMixer _audioMixer;

		public float MasterVolume {
			get => _state.MasterVolume;
			set {
				_state.MasterVolume = value;
				UpdateVolume();
				_gameState.Save();
			}
		}

		public SettingsController(GameState gameState) {
			_state     = gameState.SettingsState;
			_gameState = gameState;

			_audioMixer = Resources.Load<AudioMixerGroup>("AudioMixer").audioMixer;
			UpdateVolume();
		}

		void UpdateVolume() {
			_audioMixer.SetFloat(MasterVolumeId, MasterVolume);
		}
	}
}