using UnityEngine;

using System;

using STP.Behaviour.Starter;
using STP.Common;
using STP.Core;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

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
		public float      BulletStartForce;
		[Space]
		public float ReloadDuration;
		[Space]
		[NotNull]
		public ProgressBar HealthBar;
		[Space]
		[NotNull]
		public Collider2D ShieldCollider;

		Vector2 _input;

		Camera           _camera;
		CoreSpawnHelper  _spawnHelper;
		Transform        _playerStartPos;
		PlayerManager    _playerManager;
		LevelGoalManager _levelGoalManager;

		PlayerController _playerController;

		float _reloadTimer;

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
			_input             = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			var mouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
			Rigidbody.rotation = -Vector2.SignedAngle(mouseWorldPos - transform.position, Vector2.up);

			if ( Input.GetMouseButton(0) ) {
				TryShoot();
			}

			_reloadTimer -= Time.deltaTime;
		}

		void FixedUpdate() {
			if ( _input != Vector2.zero ) {
				Rigidbody.AddForce(_input.normalized * MovementSpeed, ForceMode2D.Impulse);
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			_camera           = Camera.main;
			_spawnHelper      = starter.SpawnHelper;
			_playerStartPos   = starter.PlayerStartPos;
			_playerManager    = starter.PlayerManager;
			_levelGoalManager = starter.LevelGoalManager;

			_playerController                =  starter.PlayerController;
			_playerController.OnCurHpChanged += OnCurHpChanged;
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
			if ( _playerController.TakeDamage(damage) ) {
				_levelGoalManager.OnPlayerDied();
			}
		}

		void Deinit() {
			_playerController.OnCurHpChanged -= OnCurHpChanged;
		}

		void OnCurHpChanged(float curHp) {
			HealthBar.Progress = (curHp / PlayerController.MaxPlayerHp);
		}

		void TryShoot() {
			if ( _reloadTimer <= 0 ) {
				Shoot();
				_reloadTimer = ReloadDuration * (_playerManager.HasActivePowerUp(PowerUpNames.IncreasedFireRate)
					? TmpIncFireRateMult
					: 1f);
			}
		}

		void Shoot() {
			var isX2DamageActive = _playerManager.HasActivePowerUp(PowerUpNames.X2Damage);
			var bulletGo = Instantiate(isX2DamageActive ? EnhancedBulletPrefab : BulletPrefab,
				transform.position, Quaternion.identity, null);
			var bullet = bulletGo.GetComponent<Bullet>();
			if ( bullet ) {
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
