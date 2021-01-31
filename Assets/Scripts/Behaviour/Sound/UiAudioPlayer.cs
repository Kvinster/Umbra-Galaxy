using UnityEngine;
using UnityEngine.Assertions;

using STP.Behaviour.Utils;
using STP.Utils;

namespace STP.Behaviour.Sound {
	public sealed class UiAudioPlayer : SingleBehaviour<UiAudioPlayer> {
		AudioSource _audioSource;

		CameraUtility _cameraUtility;

		CommonSoundsContainer _soundsContainer;

		protected override void Awake() {
			base.Awake();

			_audioSource = GetComponentInChildren<AudioSource>();
			if ( !_audioSource ) {
				_audioSource = gameObject.AddComponent<AudioSource>();
			}

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
			_audioSource.PlayOneShot(clip, volumeScale);
		}
	}
}
