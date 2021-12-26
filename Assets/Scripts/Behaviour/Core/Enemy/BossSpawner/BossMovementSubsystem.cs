using System;
using STP.Core;
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

		public float DashVelocityMultiplier = 3;

		public float MovingSpeed;
		public float AngularSpeed;

		public float DashAngularSpeed = 15;

		public float SlowdownTime = 2f;
		public float DashTime     = 2f;

		Rect _dashEndArea;

		public BaseTask DashTask => new SequenceTask(
			new CustomActionTask("set dash speed", Dash),
			new WaitTask(DashTime),
			new RepeatUntilSuccess(
				new SequenceTask(
					new ConditionTask(() => _dashEndArea.Contains(BossRigidbody.transform.position)),
					new CustomActionTask("stop dash", EndDash)
				)
			)
		);

		readonly Timer _timer = new Timer();

		float   _startAngularSpeed;
		Vector2 _startSpeed;

		bool _isDash;

		Transform _player;

		Vector2 ForwardVector => (BossRigidbody.transform.rotation * Vector3.up).normalized;

		HpSystem _hpSystem;

		bool IsActive { get; set; }

		public void Init(Rigidbody2D bossRigidbody, Transform player, HpSystem hpSystem, Rect playArea) {
			BossRigidbody    =  bossRigidbody;
			_player          =  player;
			_hpSystem        =  hpSystem;
			_hpSystem.OnDied += OnDied;
			CalcDashEndArea(playArea);
			_timer.Reset(int.MaxValue);
		}

		public void FixedUpdate() {
			if ( !IsActive ) {
				return;
			}
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
			if (_isDash) {
				LookAtObject(Vector3.zero, DashAngularSpeed);
				SetDashVelocity();
			}
			else {
				LookAtObject(_player.position, AngularSpeed);
			}
		}

		public void SetActive(bool isActive) {
			IsActive = isActive;
			BossRigidbody.bodyType = IsActive ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
			if ( IsActive ) {
				BossRigidbody.velocity = Vector2.zero;
			}
		}

		void CalcDashEndArea(Rect playArea) {
			// Restricting dash end area as half from the play area
			var center = playArea.center;
			playArea.size /= 2;
			playArea.center = center;
			_dashEndArea = playArea;
		}

		void Dash() {
			SetDashVelocity();
			_isDash                = true;
			BossRigidbody.drag     = 0f;
		}

		void EndDash() {
			_isDash = false;
		}

		void SetDashVelocity() {
			var forward = ForwardVector;
			BossRigidbody.velocity = forward * (MovingSpeed * DashVelocityMultiplier);
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
			var distance = Vector2.Distance(BossRigidbody.transform.position, _player.transform.position);
			var forward  = ForwardVector;
			BossRigidbody.drag = 0f;
			if ( distance < MinDistance ) {
				BossRigidbody.velocity = -forward * MovingSpeed;
			} else if ( distance > MaxDistance ) {
				BossRigidbody.velocity = forward * MovingSpeed;
			} else {
				BossRigidbody.drag = 3f;
			}
		}

		void LookAtObject(Vector3 objPosition, float angularSpeed) {
			var targetAngle = GetAngleToObject(objPosition);
			var currentAngle = BossRigidbody.rotation;
			var diff = targetAngle - currentAngle;
			// rotate in the shortest direction
			if ( Mathf.Abs(diff) > 180 ) {
				diff = -diff;
			}
			var angleAmplitude = Mathf.Clamp(angularSpeed * Time.deltaTime, 0, Mathf.Abs(diff));
			var newAngle = currentAngle + Mathf.Sign(diff) * angleAmplitude;
			BossRigidbody.transform.rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
		}

		float GetAngleToObject(Vector3 objPosition) {
			var diff = objPosition - BossRigidbody.transform.position;
			diff.Normalize();
			var angle = Mathf.Atan2(diff.x, diff.y) * Mathf.Rad2Deg;
			return -angle;
		}

		void OnDrawGizmos() {
			// drawing future dash area
			var color = Gizmos.color;
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(_dashEndArea.center, _dashEndArea.size);
			Gizmos.color = color;
		}
	}
}