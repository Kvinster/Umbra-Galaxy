using UnityEngine;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public class Mine : BaseEnemy {
		[Header("Parameters")]
		public float DamageOnCollision = int.MaxValue;
		public float Lifetime          = 5f;
		[Header("Dependencies")]
		[NotNull] public BaseSimpleSoundPlayer SpawnSoundPlayer;

		readonly Timer _timer = new Timer();

		void Start() {
			_timer.Reset(Lifetime);
			SpawnSoundPlayer.Play();
		}

		void Update() {
			if ( _timer.DeltaTick() ) {
				Die(false);
			}
		}

		public override void Die(bool fromPlayer = true) {
			base.Die(fromPlayer);
			Destroy(gameObject);
		}

		public void OnCollisionEnter2D(Collision2D other) {
			other.TryTakeDamage(DamageOnCollision);
			Die(false);
		}
	}
}