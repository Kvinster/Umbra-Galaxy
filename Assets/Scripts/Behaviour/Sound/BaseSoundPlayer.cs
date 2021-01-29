using UnityEngine;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	[RequireComponent(typeof(AudioSource))]
	public abstract class BaseSoundPlayer : GameComponent {
		[NotNull] public AudioSource AudioSource;

		protected virtual void Reset() {
			AudioSource = GetComponentInChildren<AudioSource>();
		}
	}
}
