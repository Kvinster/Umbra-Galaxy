using UnityEngine;

using System;

using STP.Behaviour.Starter;
using STP.Common;
using STP.Core;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using NaughtyAttributes;

namespace STP.Behaviour.Core {
	public sealed class Player : BaseCoreComponent, IDestructible {
		const float TmpBulletDamage    = 10f;
		const float TmpIncFireRateMult = 1f / 4f;

		[NotNull]
		public Rigidbody2D Rigidbody;
		[NotNull]
		public Collider2D  Collider;
		public float       MovementSpeed;
		[Space]
		[NotNull]
		public GameObject BulletPrefab;
		[NotNull]
		public GameObject EnhancedBulletPrefab;
		[NotNull]
		public Transform  GunPoint;
		public float BulletStartForce;
		[Space]
		public float ReloadDuration;
		[Space]
		[NotNull]
		public ProgressBar HealthBar;
		[Space]
		[NotNull]
		public Collider2D ShieldCollider;

		[BoxGroup("Sound")] [NotNull] public BaseSimpleSoundPlayer DeathSoundPlayer;
		[BoxGroup("Sound")] [NotNull] public BaseSimpleSoundPlayer ShotSoundPlayer;

		Vector2 _input;

		Camera           _camera;
		CoreSpawnHelper  _spawnHelper;
		Transform        _playerStartPos;
		PlayerManager    _playerManager;
		PauseManager     _pauseManager;
		LevelGoalManager _levelGoalManager;

		PlayerController _playerController;

		float _reloadTimer;

		bool IsAlive => _playerController.IsAlive;

		float CurHp => _playerController.CurHp;

		public event Action OnPlayerDied;

		void Reset() {
			Rigidbody = GetComponent<Rigidbody2D>();
		}

		protected override void OnDisable() {
			base.OnDisable();
			Deinit();
		}

		void Update() {
			if ( _pauseManager.IsPaused ) {
				return;
			}

			_input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			if ( Input.GetMouseButton(0) ) {
				TryShoot();
			}

			_reloadTimer -= Time.deltaTime;
		}

		void FixedUpdate() {
			if ( _pauseManager.IsPaused ) {
				return;
			}
			if ( _input != Vector2.zero ) {
				Rigidbody.AddForce(_input.normalized * MovementSpeed, ForceMode2D.Impulse);
			}
			var mouseWorldPos  = _camera.ScreenToWorldPoint(Input.mousePosition);
			var neededRotation = -Vector2.SignedAngle(mouseWorldPos - transform.position, Vector2.up);
			Rigidbody.MoveRotation(neededRotation);
		}

		protected override void InitInternal(CoreStarter starter) {
			_camera           = Camera.main;
			_spawnHelper      = starter.SpawnHelper;
			_playerStartPos   = starter.PlayerStartPos;
			_playerManager    = starter.PlayerManager;
			_pauseManager     = starter.PauseManager;
			_levelGoalManager = starter.LevelGoalManager;

			_playerController                  =  starter.PlayerController;
			_playerController.OnCurHpChanged   += OnCurHpChanged;
			_playerController.OnIsAliveChanged += OnIsAliveChanged;
			OnCurHpChanged(CurHp);

			HealthBar.Init(1f);

			OnRespawn();

			Physics2D.IgnoreCollision(Collider, ShieldCollider);
		}

		public void OnRespawn() {
			Rigidbody.position = _playerStartPos.position;
			Rigidbody.velocity = Vector2.zero;
			Rigidbody.rotation = 0f;
			_reloadTimer       = 0f;
		}

		public void OnRestart() {
			Deinit();
			OnPlayerDied?.Invoke();
		}

		public void TakeDamage(float damage) {
			if ( !IsAlive ) {
				return;
			}
			_playerController.TakeDamage(damage);
		}

		void OnIsAliveChanged(bool isAlive) {
			if ( !isAlive ) {
				DeathSoundPlayer.Play();
			}
		}

		void Deinit() {
			_playerController.OnCurHpChanged   -= OnCurHpChanged;
			_playerController.OnIsAliveChanged -= OnIsAliveChanged;
		}

		void OnCurHpChanged(float curHp) {
			HealthBar.Progress = (curHp / PlayerController.MaxPlayerHp);
		}

		void TryShoot() {
			if ( _reloadTimer <= 0 ) {
				Shoot();
				_reloadTimer = ReloadDuration * (_playerManager.HasActivePowerUp(PowerUpType.IncFireRate)
					? TmpIncFireRateMult
					: 1f);
				ShotSoundPlayer.Play();
			}
		}

		void Shoot() {
			var isX2DamageActive = _playerManager.HasActivePowerUp(PowerUpType.X2Damage);
			var bulletGo = Instantiate(isX2DamageActive ? EnhancedBulletPrefab : BulletPrefab,
				GunPoint.position, Quaternion.AngleAxis(Rigidbody.rotation, Vector3.forward), _spawnHelper.TempObjRoot);
			var bullet = bulletGo.GetComponent<IBullet>();
			if ( bullet != null ) {
				var mult = isX2DamageActive ? 2f : 1f;
				bullet.Init(TmpBulletDamage * mult, Vector2.up * BulletStartForce, Rigidbody.rotation, Collider,
					ShieldCollider);
			} else {
				Debug.LogErrorFormat("No Bullet component on current bullet prefab (x2 damage: '{0}')",
					isX2DamageActive);
			}
			_spawnHelper.TryInitSpawnedObject(bulletGo);
		}
	}
}
