using UnityEngine.VFX;

using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.PowerUps {
	public sealed class PowerUpEffectPlayerTracker : BaseCoreComponent {
		[NotNull] public VisualEffect VisualEffect;

		Player _player;

		protected override void InitInternal(CoreStarter starter) {
			_player = starter.Player;
		}

		void Update() {
			if ( _player ) {
				VisualEffect.SetVector3("PlayerPos", transform.InverseTransformPoint(_player.transform.position));
			}
		}
	}
}
