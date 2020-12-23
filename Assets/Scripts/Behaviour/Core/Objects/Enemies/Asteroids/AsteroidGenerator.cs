using UnityEngine;

using STP.Events;

using Random = UnityEngine.Random;

namespace STP.Behaviour.Core.Objects.Enemies.Asteroids {
	public class AsteroidGenerator : BaseObjectGenerator<FlyingAsteroidDestroyed> {
		const float MinSpeed = 300f;
		const float MaxSpeed = 500f;

		protected override void InitObject(GameObject obj) {
			var asteroid = obj.GetComponent<FlyingAsteroid>();
			asteroid.Init(PlayerShipTrans, GenerateAsteroidDirection(obj.transform.position), GenerateAsteroidSpeed());
		}

		float GenerateAsteroidSpeed() {
			return Random.Range(MinSpeed, MaxSpeed);
		}
	}
}