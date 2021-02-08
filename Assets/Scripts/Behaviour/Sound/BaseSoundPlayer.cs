using UnityEngine;

using STP.Utils;

namespace STP.Behaviour.Core {
	public abstract class BaseSoundPlayer : GameComponent {
		public AudioSource OverrideAudioSource;

		protected virtual void Reset() {
			OverrideAudioSource = GetComponentInChildren<AudioSource>();
		}
	}
}
