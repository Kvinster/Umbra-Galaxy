using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public sealed class Fighter : BaseEnemy, IDestructible {
		[Space]
		public float StartHp;
		public float MovementSpeed;
		[Range(0f, 1f)]
		public float RotationSpeed;
		[NotNull]
		public Rigidbody2D Rigidbody;
		[NotNull]
		public Collider2D  Collider;

		[Header("Gun")]
		[NotNull]
		public GameObject Bullet;
		public float      FirePeriod;
		public float      BulletStartSpeed;

		[Header("Sound")]
		[NotNull]
		public BaseSimpleSoundPlayer ShotSoundPlayer;

		readonly Timer _fireTimer = new Timer();

		Transform       _target;
		CoreSpawnHelper _spawnHelper;

		float CurHp { get; set; }

		void Update() {
			if ( !_target ) {
				return;
			}
			if ( _fireTimer.DeltaTick() ) {
				Fire();
			}
			var dirRaw = _target.position - transform.position;
			Rigidbody.rotation += MathUtils.GetSmoothRotationAngleOffset(transform.up, dirRaw, RotationSpeed);
		}

		void FixedUpdate() {
			Rigidbody.MovePosition(transform.position + transform.up * (MovementSpeed * Time.fixedDeltaTime));
		}

		void OnCollisionEnter2D(Collision2D other) {
			var destructible = other.gameObject.GetComponent<IDestructible>();
			if ( destructible != null ) {
				destructible.TakeDamage(20);
				Die();
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);
			_spawnHelper = starter.SpawnHelper;
			CurHp        = StartHp;
			_fireTimer.Start(FirePeriod);
		}

		public void TakeDamage(float damage) {
			CurHp = Mathf.Max(CurHp - damage, 0);
			if ( CurHp == 0 ) {
				Die();
			}
		}

		protected override void Die(bool fromPlayer = true) {
			base.Die(fromPlayer);

			Destroy(gameObject);
		}


		public override void OnBecomeVisibleForPlayer(Transform playerTransform) {
			SetTarget(playerTransform);
		}

		public override void OnBecomeInvisibleForPlayer() {
			// Do nothing
		}
		
		public override void SetTarget(Transform target) {
			_target = target;
		}

		void Fire() {
			var go = Instantiate(Bullet, transform.position, Quaternion.identity, _spawnHelper.TempObjRoot);
			var bullet = go.GetComponent<IBullet>();
			if ( bullet == null ) {
				Debug.LogError("Can't init bullet in fighter. Component Bullet not found");
				Destroy(go);
				return;
			}
			bullet.Init(10f, BulletStartSpeed, transform.rotation.eulerAngles.z, Collider);
			_spawnHelper.TryInitSpawnedObject(go);
			ShotSoundPlayer.Play();
		}
	}
}
