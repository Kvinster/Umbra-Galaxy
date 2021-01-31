using UnityEngine;

using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public sealed class SimpleSoundPlayer : BaseSimpleSoundPlayer {
		[NotNull(false)]
		public AudioClip AudioClip;
		public float     VolumeScale = 1f;

		public override void Play() {
			AudioSource.PlayOneShot(AudioClip, VolumeScale);
		}
	}
}
