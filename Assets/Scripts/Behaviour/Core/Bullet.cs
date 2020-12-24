using UnityEngine;

namespace STP.Behaviour.Core {
	public sealed class Bullet : MonoBehaviour {
		public float LifeTime = 3f;

		float _lifeTimer;

		void Update() {
			_lifeTimer += Time.deltaTime;
			if ( _lifeTimer >= LifeTime ) {
				Destroy(gameObject);
			}
		}

		void OnCollisionEnter2D(Collision2D other) {
			Destroy(gameObject);
		}
	}
}
