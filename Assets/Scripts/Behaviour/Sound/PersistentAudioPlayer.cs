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

			_musicAudioSource                       = gameObject.AddComponent<AudioSource>();
			_musicAudioSource.outputAudioMixerGroup = Resources.Load<AudioMixerGroup>("AudioMixer");
			_musicAudioSource.loop                  = true;

			_soundAudioSource                       = gameObject.AddComponent<AudioSource>();
			_soundAudioSource.outputAudioMixerGroup = Resources.Load<AudioMixerGroup>("AudioMixer");
			_soundAudioSource.loop                  = false;

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

		public void SetPitch(float pitch) {
			pitch                   = Mathf.Clamp01(pitch);
			_musicAudioSource.pitch = pitch;
			_soundAudioSource.pitch = pitch;
		}
	}
}