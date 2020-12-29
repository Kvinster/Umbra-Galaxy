using UnityEngine;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public sealed class Bullet : GameComponent {
		[NotNull] 
		public Rigidbody2D Rigidbody;
		[NotNull] 
		public Collider2D  Collider;
		
		public float LifeTime = 3f;

		float _lifeTimer;
       
		public void Init(Collider2D ownerCollider, Vector2 force, float rotation) {
			Rigidbody.rotation = rotation;
			Rigidbody.AddRelativeForce(force, ForceMode2D.Impulse);
			Physics2D.IgnoreCollision(ownerCollider, Collider);
		}
		
		void Update() {
			_lifeTimer += Time.deltaTime;
			if ( _lifeTimer >= LifeTime ) {
				Destroy(gameObject);
			}
		}

		void OnCollisionEnter2D(Collision2D other) {
			var destructible = other.gameObject.GetComponent<IDestructible>();
			destructible?.TakeDamage(10f);
			Destroy(gameObject);
		}
	}
}
