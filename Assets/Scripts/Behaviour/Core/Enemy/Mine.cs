using STP.Utils;
using UnityEngine;

namespace STP.Behaviour.Core.Enemy {
	public class Mine : BaseEnemy {
		public float DamageOnCollision = int.MaxValue;
		public float Lifetime          = 5f;

		Timer _timer = new Timer();
		
		void Start() {
			_timer.Reset(Lifetime);
		}

		void Update() {
			if ( _timer.DeltaTick() ) {
				Die(false);
				Destroy(gameObject);
			}
		}
		
		public void OnCollisionEnter2D(Collision2D other) {
			other.TryTakeDamage(DamageOnCollision);
		}
	}
}