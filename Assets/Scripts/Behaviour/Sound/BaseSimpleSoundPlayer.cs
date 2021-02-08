using UnityEngine;

using STP.Behaviour.Sound;

namespace STP.Behaviour.Core {
	public abstract class BaseSimpleSoundPlayer : BaseSoundPlayer {
		public abstract void Play();

		protected void PlayOneShot(AudioClip clip, float volumeScale = 1f) {
			if ( OverrideAudioSource ) {
				OverrideAudioSource.PlayOneShot(clip, volumeScale);
			} else {
				PersistentAudioPlayer.Instance.PlayOneShot(clip, volumeScale);
			}
		}
	}
}
