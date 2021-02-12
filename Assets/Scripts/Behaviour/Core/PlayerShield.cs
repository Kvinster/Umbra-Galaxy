using UnityEngine;
using UnityEngine.VFX;

using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	[RequireComponent(typeof(Collider2D))]
	public sealed class PlayerShield : BaseCoreComponent {
		[NotNull] public BaseSimpleSoundPlayer ImpactSoundPlayer;
		[NotNull] public VisualEffect          VisualEffect;

		protected override void InitInternal(CoreStarter starter) {
		}

		void OnCollisionEnter2D(Collision2D other) {
			var worldContact = other.contacts[0].point;
			VisualEffect.SetVector2("CollisionPos", VisualEffect.transform.InverseTransformPoint(worldContact));
			VisualEffect.SetVector2("CollisionDirection", (worldContact - (Vector2) transform.position).normalized);
			VisualEffect.SendEvent("OnCollision");

			ImpactSoundPlayer.Play();
		}
	}
}
