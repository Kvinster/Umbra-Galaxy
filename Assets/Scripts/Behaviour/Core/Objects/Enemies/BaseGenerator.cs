using UnityEngine;

using STP.Behaviour.Starter;
using STP.Events;
using STP.Utils;
using STP.Utils.Events;
using STP.Utils.GameComponentAttributes;

using Random = UnityEngine.Random;

namespace STP.Behaviour.Core.Objects.Enemies {
	public abstract class BaseObjectGenerator<TObjectDestroyedEvent> : BaseCoreComponent where TObjectDestroyedEvent : struct {
		const float SpawnMinTimeSec = 0.3f;
		const float SpawnMaxTimeSec = 2;
		
		const int MaxObjectAmount = 10;

		const float StartDistanceFromPlayer = 1000f;
		const float MinSpeed = 300f;
		const float MaxSpeed = 500f;
		
		[NotNull] public GameObject Prefab;
		
		protected Transform  PlayerShipTrans;
		
		readonly Timer _timer = new Timer();
		
		int _currentObjectsAmount;
		
		protected override void InitInternal(CoreStarter starter) {
			PlayerShipTrans = starter.Player;
			EventManager.Subscribe<TObjectDestroyedEvent>(OnFlyingAsteroidDestroyed);
			EventManager.Subscribe<PlayerDestroyed>     (OnPlayerDestroyed);
		}

		void OnDestroy() {
			EventManager.Unsubscribe<TObjectDestroyedEvent>(OnFlyingAsteroidDestroyed);
			EventManager.Unsubscribe<PlayerDestroyed>     (OnPlayerDestroyed);
		}

		void Update() {
			if ( _timer.DeltaTick() && (_currentObjectsAmount < MaxObjectAmount) ) {
				GenerateObject();
			}
		}

		protected abstract void InitObject(GameObject obj);  
		
		protected Vector2 GenerateAsteroidDirection(Vector3 asteroidPos) {
			var xDir = Random.Range(-1, 1);
			var yDir = Random.Range(-1, 1);
			var res = new Vector2(xDir, yDir);
			if ( res == Vector2.zero ) {
				res = Vector2.right;
			}
			var directionToPlayer = (PlayerShipTrans.position - asteroidPos).normalized;
			// Detecting and fixing direction. The velocity vector cannot be directed in the opposite direction from the player
			var angleBetweenVectors = Vector2.SignedAngle(directionToPlayer, res);
			if ( (angleBetweenVectors > 90) || (angleBetweenVectors < -90) ) {
				res = -res;
			}
			return res;
		}
		
		void OnPlayerDestroyed(PlayerDestroyed e) {
			// Stop generator
			_timer.Stop();
		}
		
		void OnFlyingAsteroidDestroyed(TObjectDestroyedEvent e) {
			_currentObjectsAmount--;
		}
		
		void GenerateObject() {
			var obj = Instantiate(Prefab);
			obj.transform.position = GenerateObjectPosition();
			InitObject(obj);
			// Generating next object time
			var newSpawnDelay = Random.Range(SpawnMinTimeSec, SpawnMaxTimeSec);
			_timer.Reset(newSpawnDelay);
			_currentObjectsAmount++;
		}

		Vector3 GenerateObjectPosition() {
			var center = PlayerShipTrans.position;
			// Getting a random point on a circle with radius = StartDistanceFromPlayer
			var xOffset = Random.Range(-StartDistanceFromPlayer, StartDistanceFromPlayer);
			var yOffset = Mathf.Sqrt(StartDistanceFromPlayer * StartDistanceFromPlayer - xOffset * xOffset);
			var invertY = Random.Range(0, 2);
			if ( invertY != 0 ) {
				yOffset = -yOffset;
			}
			var offset  = new Vector3(xOffset, yOffset);
			return center + offset;
		}

		Vector2 GenerateObjectDirection(Vector3 asteroidPos) {
			var xDir = Random.Range(-1, 1);
			var yDir = Random.Range(-1, 1);
			var res = new Vector2(xDir, yDir);
			if ( res == Vector2.zero ) {
				res = Vector2.right;
			}
			var directionToPlayer = (PlayerShipTrans.position - asteroidPos).normalized;
			// Detecting and fixing direction. The velocity vector cannot be directed in the opposite direction from the player
			var angleBetweenVectors = Vector2.SignedAngle(directionToPlayer, res);
			if ( (angleBetweenVectors > 90) || (angleBetweenVectors < -90) ) {
				res = -res;
			}
			return res;
		}

		float GenerateAsteroidSpeed() {
			return Random.Range(MinSpeed, MaxSpeed);
		}
	}
}