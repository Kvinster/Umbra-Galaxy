﻿using STP.Core;
using STP.Utils;
using STP.Utils.BehaviourTree;
using STP.Utils.BehaviourTree.Tasks;
using STP.Utils.GameComponentAttributes;
using UnityEngine;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class BossMovementSubsystem : GameComponent {
		[NotNull] public Rigidbody2D BossRigidbody;

		public float MinDistance = 100;
		public float MaxDistance = 100;

		public float MovingSpeed;
		public float AngularSpeed;

		public float SlowdownTime = 2f;
		public float DashTime     = 2f;

		public BaseTask DashTask => new SequenceTask(
			new CustomActionTask("set dash speed", Dash),
			new WaitTask(DashTime),
			new CustomActionTask("stop dash", EndDash)
		);
		


		readonly Timer _timer = new Timer();
		
		float   _startAngularSpeed;
		Vector2 _startSpeed;

		bool _isDash;
		
		Transform _player;

		Vector2 ForwardVector => (BossRigidbody.transform.rotation * Vector3.up).normalized;

		HpSystem _hpSystem;
		
		public void Init(Rigidbody2D bossRigidbody, Transform player, HpSystem hpSystem) {
			BossRigidbody    =  bossRigidbody;
			_player          =  player;
			_hpSystem        =  hpSystem;
			_hpSystem.OnDied += OnDied;
			_timer.Reset(int.MaxValue);
		}
		
		public void FixedUpdate() {
			if ( !_hpSystem.IsAlive ) {
				if ( _timer.DeltaTick() ) {
					BossRigidbody.bodyType = RigidbodyType2D.Static;
					_timer.Reset(int.MaxValue);
					enabled = false;
					return;
				}
				Slowdown();
			}
			else {
				KeepDistanceFromPlayer();
			}
			LookToPlayer();
		}

		
		public void Dash() {
			var forward = ForwardVector;
			BossRigidbody.velocity = forward * MovingSpeed * 3;
			_isDash                = true;
		}

		public void EndDash() {
			_isDash = false;
		}
		
		void OnDied() {
			_timer.Reset(SlowdownTime);
			_startSpeed        =  BossRigidbody.velocity;
			_startAngularSpeed =  (!_isDash) ? AngularSpeed : 0f;
			_hpSystem.OnDied  -= OnDied;
		}

		void Slowdown() {
			AngularSpeed           = Mathf.Lerp(_startAngularSpeed, 0f, _timer.NormalizedProgress);
			BossRigidbody.velocity = Vector2.Lerp(_startSpeed, Vector2.zero, _timer.NormalizedProgress);
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