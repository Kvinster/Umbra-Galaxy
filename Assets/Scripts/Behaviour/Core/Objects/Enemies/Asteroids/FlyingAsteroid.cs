using UnityEngine;

using STP.Events;
using STP.Gameplay;
using STP.Utils;
using STP.Utils.Events;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Objects.Enemies.Asteroids {
	public class FlyingAsteroid : GameComponent {
		const float MaxDistanceToPlayer = 1200;
		
		[NotNull] public Rigidbody2D Rigidbody2D;

		Transform _playerShipTrans;

		float DistanceToPlayerShip => (_playerShipTrans.position - transform.position).magnitude;
		bool OutOfPlayerRange => DistanceToPlayerShip > MaxDistanceToPlayer;
		
		public void Init(Transform playerShipTrans, Vector2 flyingDirection, float flyingSpeed) {
			Rigidbody2D.velocity = flyingDirection * flyingSpeed;
			_playerShipTrans = playerShipTrans;
			EventManager.Subscribe<PlayerDestroyed>(OnPlayerDestroyed);
		}
		
		public void OnCollisionEnter2D(Collision2D other) {
			var destructable = other.gameObject.GetComponent<IDestructable>();
			destructable?.GetDamage(float.MaxValue);
		}
		void Update() {
			if ( OutOfPlayerRange ) {
				DestroyAsteroid();
			}
		}

		void OnDestroy() {
			EventManager.Unsubscribe<PlayerDestroyed>(OnPlayerDestroyed);
		}
		
		void OnPlayerDestroyed(PlayerDestroyed e) {
			DestroyAsteroid();
		}

		void DestroyAsteroid() {
			EventManager.Fire(new FlyingAsteroidDestroyed());
			Destroy(gameObject);
		}
	}
}