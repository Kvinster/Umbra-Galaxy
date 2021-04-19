using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public sealed class Glider : BaseEnemy, IDestructible {
		[Space]
		public float           StartHp = 20;
		public float           MinAttackDistance;
		public float           MaxAttackDistance;
		public float           MovementSpeed;
		public float           RotationSpeed;
		[NotNull]
		public Collider2D      Collider;
		public Rigidbody2D     Rigidbody;
		public TriggerNotifier DetectRangeNotifier;
		public float           ShootInterval;
		public GameObject      BulletPrefab;
		public float           BulletStartSpeed;
		[Header("Sound")]
		[NotNull]
		public BaseSimpleSoundPlayer ShotSoundPlayer;

		Transform _target;

		CoreSpawnHelper _spawnHelper;

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
			var bulletGo = Instantiate(BulletPrefab, transform.position, Quaternion.identity, _spawnHelper.TempObjRoot);
			var bullet   = bulletGo.GetComponent<IBullet>();
			if ( bullet != null ) {
				bullet.Init(10f, BulletStartSpeed, Rigidbody.rotation, Collider);
			} else {
				Debug.LogError("No Bullet component on BulletPrefab");
				return false;
			}
			_spawnHelper.TryInitSpawnedObject(bulletGo);
			ShotSoundPlayer.Play();
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
			base.InitInternal(starter);
			_spawnHelper = starter.SpawnHelper;

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

		protected override void Die(bool fromPlayer = true) {
			base.Die(fromPlayer);
			DetectRangeNotifier.OnTriggerEnter -= OnDetectRangeEnter;
			DetectRangeNotifier.OnTriggerExit  -= OnDetectRangeExit;

			Destroy(gameObject);
		}

		public override void OnBecomeVisibleForPlayer(Transform playerTransform) {
			SetTarget(playerTransform);
		}

		public override void OnBecomeInvisibleForPlayer() {
			
		}

		public override void SetTarget(Transform target) {
			_target = target;
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
