using UnityEngine;

using STP.Behaviour.Core.Enemy;
using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

using Shapes;

namespace STP.Behaviour.Core {
	public sealed class LevelWinExplosionZone : BaseCoreComponent {
		[Header("Parameters")]
		public float StartRadius;
		public float MaxRadius;
		public float GrowSpeed;
		[Header("Dependencies")]
		[NotNull] public CircleCollider2D Collider;

		public Disc Disc;

		bool _stoppedGrow;

		protected override void InitInternal(CoreStarter starter) {
			Collider.radius = StartRadius;
			if ( Disc ) {
				Disc.Radius = StartRadius;
			}
		}

		void FixedUpdate() {
			if ( _stoppedGrow ) {
				return;
			}
			var radius = Collider.radius + Time.fixedDeltaTime * GrowSpeed;
			Collider.radius = radius;
			if ( Disc ) {
				Disc.Radius = radius;
			}
			if ( radius >= MaxRadius ) {
				_stoppedGrow = true;
			}
		}

		public void SetActive(bool isActive) {
			gameObject.SetActive(isActive);
		}

		void OnTriggerEnter2D(Collider2D other) {
			if ( other.TryGetComponent<BaseEnemy>(out var enemy) ) {
				if ( enemy.IsAlive ) {
					enemy.Die(false);
				}
			}
			if ( other.TryGetComponent<IBullet>(out var bullet) ) {
				bullet.Die();
			}
			if ( other.TryGetComponent<Asteroid>(out var asteroid) ) {
				asteroid.Die(false);
			}
		}
	}
}
