using UnityEngine;

namespace STP.Behaviour.Core.Enemy.Spawners {
	public class AsteroidSpawner : BaseSpawner {
		public float AsteroidSpeed;

		protected override void InitItem(GameObject go) {
			var asteroid = go.GetComponent<Asteroid>();
			if ( !asteroid ) {
				Debug.LogError("Can't init asteroid");
				return;
			}
			var dirToPlayer = Player.transform.position - go.transform.position;
			asteroid.Init(dirToPlayer.normalized, AsteroidSpeed);
		}
	}
}