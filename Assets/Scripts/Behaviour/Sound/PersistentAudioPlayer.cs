using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

using STP.Behaviour.Utils;
using STP.Utils;

namespace STP.Behaviour.Sound {
	public sealed class PersistentAudioPlayer : SingleBehaviour<PersistentAudioPlayer> {
		AudioSource _musicAudioSource;
		AudioSource _soundAudioSource;

		CameraUtility _cameraUtility;

		CommonSoundsContainer _soundsContainer;

		protected override void Awake() {
			base.Awake();

			var audioMixer = Resources.Load<AudioMixerGroup>("AudioMixer").audioMixer;
			Assert.IsTrue(audioMixer);

			_musicAudioSource                       = gameObject.AddComponent<AudioSource>();
			_musicAudioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("BGM")[0];
			_musicAudioSource.loop                  = true;
			Assert.IsTrue(_musicAudioSource.outputAudioMixerGroup);

			_soundAudioSource                       = gameObject.AddComponent<AudioSource>();
			_soundAudioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
			_soundAudioSource.loop                  = false;
			Assert.IsTrue(_soundAudioSource.outputAudioMixerGroup);

			_cameraUtility = CameraUtility.Instance;

			_soundsContainer = Resources.Load<CommonSoundsContainer>("CommonSoundsContainer");
			Assert.IsTrue(_soundsContainer);
		}

		void Update() {
			transform.position = _cameraUtility.Camera.transform.position;
		}

		public void PlayUiClick() {
			PlayOneShot(_soundsContainer.UiClickClip);
		}

		public void PlayOneShot(AudioClip clip, float volumeScale = 1f) {
			if ( !clip ) {
				Debug.LogError("Clip is null");
				return;
			}
			_soundAudioSource.PlayOneShot(clip, volumeScale);
		}

		public void PlayMusic(AudioClip clip, float volumeScale = 1f) {
			if ( !clip ) {
				Debug.LogError("Clip is null");
				return;
			}
			if ( _musicAudioSource.clip == clip ) {
				return;
			}
			_musicAudioSource.volume = volumeScale;
			_musicAudioSource.clip   = clip;
			_musicAudioSource.Play();
		}
	}
}
