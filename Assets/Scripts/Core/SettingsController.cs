﻿using UnityEngine;
using UnityEngine.Audio;

using STP.Core.State;

namespace STP.Core {
	public class SettingsController : BaseStateController {
		public const string MasterVolumeId = "MasterVolume";
		public const string MusicVolumeId  = "MusicVolume";
		public const string SfxVolumeId    = "SfxVolume";

		readonly GameState     _gameState;
		readonly SettingsState _state;

		readonly AudioMixer _audioMixer;

		public float MasterVolume {
			get => _state.MasterVolume;
			set {
				if ( Mathf.Approximately(MasterVolume, value) ) {
					return;
				}
				_state.MasterVolume = value;
				UpdateVolume();
				_gameState.Save();
			}
		}

		public float MusicVolume {
			get => _state.MusicVolume;
			set {
				if ( Mathf.Approximately(MusicVolume, value) ) {
					return;
				}
				_state.MusicVolume = value;
				UpdateVolume();
				_gameState.Save();
			}
		}

		public float SfxVolume {
			get => _state.SfxVolume;
			set {
				if ( Mathf.Approximately(SfxVolume, value) ) {
					return;
				}
				_state.SfxVolume = value;
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
			_audioMixer.SetFloat(MusicVolumeId, MusicVolume);
			_audioMixer.SetFloat(SfxVolumeId, SfxVolume);
		}
	}
}