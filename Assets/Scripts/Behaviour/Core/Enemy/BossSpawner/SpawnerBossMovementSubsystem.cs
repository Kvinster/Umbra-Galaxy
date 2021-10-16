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

		bool _isDash;
		
		Transform _player;

		MovementType _activeMovementType;
		
		Vector2 ForwardVector => (BossRigidbody.transform.rotation * Vector3.up).normalized;
		
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

		
		public void Dash() {
			var forward = ForwardVector;
			BossRigidbody.velocity = forward * MovingSpeed * 3;
			_isDash                = true;
		}

		public void EndDash() {
			_isDash = false;
		}
		
		void KeepDistanceFromPlayer() {
			if ( _isDash ) {
				return;
			}
			var distance = BossRigidbody.transform.position - _player.transform.position;
			var forward  = ForwardVector;
			if ( distance.magnitude < MinDistance ) {
				BossRigidbody.velocity = -forward * MovingSpeed / distance;
				return;
			} 
			if ( distance.magnitude > MaxDistance ) {
				BossRigidbody.velocity = forward * MovingSpeed;
				return;
			}
		}

		void LookToPlayer() {
			if ( _isDash ) {
				return;
			}
			var targetAngle       = GetAngleToPlayer();
			var oldAngle          = BossRigidbody.rotation;
			var diff              = targetAngle - oldAngle;
			if ( Mathf.Abs(diff) > 180 ) {
				diff = -diff;
			}
			var movementAmplitude = Mathf.Clamp(AngularSpeed * Time.deltaTime, 0, Mathf.Abs(diff));
			var newAngle          = oldAngle + Mathf.Sign(diff) * movementAmplitude;
			BossRigidbody.transform.rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
		}
		
		float GetAngleToPlayer() {
			var diff = _player.position - BossRigidbody.transform.position;
			diff.Normalize();
			var angle = Mathf.Atan2(diff.x, diff.y) * Mathf.Rad2Deg;
			return -angle;
		}
	}
}