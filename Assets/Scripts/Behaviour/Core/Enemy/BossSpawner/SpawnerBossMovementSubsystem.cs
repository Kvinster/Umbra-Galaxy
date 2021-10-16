using System;
using STP.Utils;
using STP.Utils.BehaviourTree;
using STP.Utils.BehaviourTree.Tasks;
using STP.Utils.GameComponentAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public enum MovementType {
		SpinUp,
		SpinDown,
		ChargeDash,
		Dash,
		Nothing,
	}
	
	public class SpawnerBossMovementSubsystem : GameComponent {
		[NotNull] public Rigidbody2D BossRigidbody;

		public float MinDistance = 100;
		public float MaxDistance = 100;

		public float MovingSpeed;
		public float AngularSpeed;
		
		
		Transform _player;

		MovementType _activeMovementType;
		
		
		public void Init(Rigidbody2D bossRigidbody, Transform player) {
			BossRigidbody = bossRigidbody;
			_player        = player;
		}

		public void SetMovementType(MovementType movementType) {
			_activeMovementType = movementType;
		}
		
		public void FixedUpdate() {
			LookToPlayer();
			KeepDistanceFromPlayer();
		}

		void KeepDistanceFromPlayer() {
			var distance   = BossRigidbody.transform.position - _player.transform.position;
			var moveVector = (BossRigidbody.transform.rotation * Vector3.up).normalized;
			if ( distance.magnitude < MinDistance ) {
				BossRigidbody.velocity = -moveVector * MovingSpeed;
				return;
			} 
			if ( distance.magnitude > MaxDistance ) {
				BossRigidbody.velocity = moveVector * MovingSpeed;
				return;
			}

			BossRigidbody.velocity = moveVector * MovingSpeed / 10;

		}

		void LookToPlayer() {
			var targetAngle       = GetAngleToPlayer();
			var oldAngle          = BossRigidbody.rotation;
			var diff              = targetAngle - oldAngle;
			if ( Mathf.Abs(diff) > 180 ) {
				diff = -diff;
			}
			var movementAmplitude = Mathf.Clamp(AngularSpeed * Time.deltaTime, 0, Mathf.Abs(diff));
			var newAngle          = oldAngle + Mathf.Sign(diff) * movementAmplitude;
			BossRigidbody.rotation = newAngle;
		}
		
		float GetAngleToPlayer() {
			var diff = _player.position - BossRigidbody.transform.position;
			diff.Normalize();
			var angle = Mathf.Atan2(diff.x, diff.y) * Mathf.Rad2Deg;
			return -angle;
		}
	}
}