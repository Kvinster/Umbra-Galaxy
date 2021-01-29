using UnityEngine;

using System.Collections.Generic;

using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public sealed class RandomSoundPlayer : BaseSimpleSoundPlayer {
		[NotNullOrEmpty] public List<AudioClip> Clips = new List<AudioClip>();

		public override void Play() {
			AudioSource.PlayOneShot(Clips[Random.Range(0, Clips.Count)]);
		}
	}
}
