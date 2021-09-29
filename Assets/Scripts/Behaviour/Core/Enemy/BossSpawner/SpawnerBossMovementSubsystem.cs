﻿using STP.Utils;
using STP.Utils.BehaviourTree;
using STP.Utils.BehaviourTree.Tasks;
using STP.Utils.GameComponentAttributes;
using UnityEngine;

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

		public float RotationAccel;
		public float MovementAccel;
		
		
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
			if ( _activeMovementType == MovementType.SpinUp ) {
				BossRigidbody.AddTorque(RotationAccel, ForceMode2D.Impulse);
			}
			if ( _activeMovementType == MovementType.SpinDown ) {
				BossRigidbody.AddTorque(-RotationAccel, ForceMode2D.Impulse);
			}
			if ( _activeMovementType == MovementType.Dash ) {
				var toPlayerDir = (_player.position - BossRigidbody.transform.position).normalized;
				BossRigidbody.AddForce(toPlayerDir * MovementAccel, ForceMode2D.Impulse);
				_activeMovementType = MovementType.Nothing;
			}
			if ( _activeMovementType == MovementType.ChargeDash ) {
				if ( BossRigidbody.angularVelocity != 0f ) {
					BossRigidbody.angularVelocity = 0f;
				}
			}
		}

		float GetAngleToPlayer() {
			var diff = _player.position - BossRigidbody.transform.position;
			diff.Normalize();
			var angle = Mathf.Atan2(diff.x, diff.y) * Mathf.Rad2Deg;
			return angle;
		}
	}
}