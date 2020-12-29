using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public sealed class Glider : BaseCoreComponent, IDestructible {
		public float           StartHp = 20;
		public float           MinAttackDistance;
		public float           MaxAttackDistance;
		public float           MovementSpeed;
		public float           RotationSpeed;
		[NotNull]public Collider2D      Collider;
		public Rigidbody2D     Rigidbody;
		public TriggerNotifier DetectRangeNotifier;
		public float           ShootInterval;
		public GameObject      BulletPrefab;
		public float           BulletStartForce;

		Transform _target;

		bool _rotateClockwise;

		float _reloadTimer;

		bool CanShoot {
			get {
				if ( !_target ) {
					return false;
				}
				if ( _reloadTimer > 0f ) {
					return false;
				}
				var distance = Vector2.Distance(_target.position, Rigidbody.position);
				return (distance <= MaxAttackDistance) && (distance >= MinAttackDistance);
			}
		}

		float CurHp { get; set; }

		void Update() {
			_reloadTimer -= Time.deltaTime;
			if ( TryShoot() ) {
				_reloadTimer = ShootInterval;
			}
		}

		bool TryShoot() {
			if ( !CanShoot ) {
				return false;
			}
			var bulletGo = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
			var bullet   = bulletGo.GetComponent<Bullet>();
			if ( bullet ) {
				bullet.Init(Collider, Vector2.up * BulletStartForce, Rigidbody.rotation);
			} else {
				Debug.LogError("No Bullet component on BulletPrefab");
				return false;
			}
			return true;
		}

		void FixedUpdate() {
			if ( !_target ) {
				return;
			}
			var targetPos = (Vector2) _target.position;
			var dirRaw    = targetPos - Rigidbody.position;
			Rigidbody.rotation += MathUtils.LerpFloat(0f, Vector2.SignedAngle(transform.up, dirRaw), RotationSpeed);

			var distance = Vector2.Distance(targetPos, Rigidbody.position);
			var curAngle = Vector2.SignedAngle(Vector2.right, (Rigidbody.position - targetPos));
			var nextAngle = (curAngle + RotationSpeed * Time.fixedDeltaTime * (_rotateClockwise ? -1 : 1)) *
			                Mathf.Deg2Rad;
			if ( distance > MaxAttackDistance ) {
				distance -= 0.5f * MovementSpeed * Time.fixedDeltaTime;
			} else if ( distance < MinAttackDistance ) {
				distance += 0.5f * MovementSpeed * Time.fixedDeltaTime;
			}
			Rigidbody.MovePosition(targetPos + new Vector2(Mathf.Cos(nextAngle), Mathf.Sin(nextAngle)) * distance);
		}

		protected override void InitInternal(CoreStarter starter) {
			CurHp = StartHp;

			_rotateClockwise = (Random.Range(0, 2) == 1);

			DetectRangeNotifier.OnTriggerEnter += OnDetectRangeEnter;
			DetectRangeNotifier.OnTriggerExit  += OnDetectRangeExit;
		}

		public void TakeDamage(float damage) {
			CurHp = Mathf.Max(CurHp - damage, 0);
			if ( CurHp == 0 ) {
				Die();
			}
		}

		void Die() {
			DetectRangeNotifier.OnTriggerEnter -= OnDetectRangeEnter;
			DetectRangeNotifier.OnTriggerExit  -= OnDetectRangeExit;

			Destroy(gameObject);
		}

		void OnDetectRangeEnter(GameObject other) {
			if ( other.GetComponent<Player>() ) {
				_target = other.transform;
			}
		}

		void OnDetectRangeExit(GameObject other) {
			if ( _target && (other.transform == _target) ) {
				_target = null;
			}
		}

		void OnCollisionEnter2D(Collision2D other) {
			var player = other.gameObject.GetComponent<Player>();
			if ( player ) {
				player.TakeDamage(20);
				Die();
			}
		}
	}
}
