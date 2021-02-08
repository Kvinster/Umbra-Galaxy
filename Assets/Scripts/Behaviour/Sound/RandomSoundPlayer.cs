using UnityEngine;

using System.Collections.Generic;

using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public sealed class RandomSoundPlayer : BaseSimpleSoundPlayer {
		public float VolumeScale = 1f;
		[Space] [NotNullOrEmpty]
		public List<AudioClip> Clips = new List<AudioClip>();

		public override void Play() {
			PlayOneShot(Clips[Random.Range(0, Clips.Count)], VolumeScale);
		}
	}
}
