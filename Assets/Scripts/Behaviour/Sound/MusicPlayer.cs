using UnityEngine;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Sound {
	public sealed class MusicPlayer : GameComponent {
		[NotNull]
		public AudioClip MusicClip;
		public float     VolumeScale = 1f;

		public void Play() {
			PersistentAudioPlayer.Instance.PlayMusic(MusicClip, VolumeScale);
		}
	}
}
