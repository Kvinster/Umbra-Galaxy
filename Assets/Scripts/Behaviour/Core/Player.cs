using UnityEngine;
using UnityEngine.VFX;

using System;

using STP.Behaviour.Starter;
using STP.Behaviour.Utils.ProgressBar;
using STP.Common;
using STP.Core;
using STP.Core.ShootingsSystems;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using Cysharp.Threading.Tasks;
using NaughtyAttributes;

namespace STP.Behaviour.Core {
	public sealed class Player : BaseCoreComponent, IDestructible {
		const float TmpIncFireRateMult = 4f;
		[NotNull]
		public ShootingSystemParams DefaultShootingParams;
		[NotNull]
		public Rigidbody2D Rigidbody;
		[NotNull]
		public Collider2D Collider;
		[Space]
		[NotNull]
		public BaseProgressBar HealthBar;
		[Space]
		[NotNull]
		public Collider2D ShieldCollider;
		[Space]
		[NotNull]
		public GameObject   DeathVisualEffectRoot;
		public VisualEffect DeathVisualEffect;
		[Space]
		[NotNull]
		public PlayerDeathAnimationController PlayerDeathAnimationController;

		[BoxGroup("Sound")] [NotNull] public BaseSimpleSoundPlayer DamageSoundPlayer;
		[BoxGroup("Sound")] [NotNull] public BaseSimpleSoundPlayer DeathSoundPlayer;
		[BoxGroup("Sound")] [NotNull] public BaseSimpleSoundPlayer ShotSoundPlayer;

		Vector2 _input;

		DefaultShootingSystem _defaultShootingSystem;
		ShootingSystemParams  _actualParams;

		Camera             _camera;
		CoreSpawnHelper    _spawnHelper;
		Transform          _playerStartPos;
		PlayerManager      _playerManager;
		LevelGoalManager   _levelGoalManager;
		PauseManager       _pauseManager;
		PrefabsController  _prefabsController;
		XpController       _xpController;
		UpgradesController _upgradesController;
		PlayerController   _playerController;

		float _movementSpeed;

		HpSystem _playerHpSystem;

		public event Action OnPlayerTakeDamage;
		public event Action OnPlayerRespawn;
		public event Action OnPlayerDied;

		void Reset() {
			Rigidbody = GetComponent<Rigidbody2D>();
		}

		protected override void OnDisable() {
			base.OnDisable();
			Deinit();
		}

		void Update() {
			if ( !IsInit || !_playerHpSystem.IsAlive ) {
				return;
			}
			if ( _pauseManager.IsPaused ) {
				return;
			}

			_input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			_defaultShootingSystem.DeltaTick();
			if ( Input.GetMouseButton(0) ) {
				TryShoot();
			}
		}

		void FixedUpdate() {
			if ( !IsInit || !_playerHpSystem.IsAlive ) {
				return;
			}
			if ( _pauseManager.IsPaused ) {
				return;
			}
			if ( _input != Vector2.zero ) {
				Rigidbody.AddForce(_input.normalized * _movementSpeed, ForceMode2D.Impulse);
			}
			var mouseWorldPos  = _camera.ScreenToWorldPoint(Input.mousePosition);
			var neededRotation = -Vector2.SignedAngle(mouseWorldPos - transform.position, Vector2.up);
			Rigidbody.MoveRotation(neededRotation);
		}

		void OnDestroy() {
			Deinit();
		}

		protected override void InitInternal(CoreStarter starter) {
			_camera             = starter.MainCamera;
			_spawnHelper        = starter.SpawnHelper;
			_playerStartPos     = starter.PlayerStartPos;
			_playerManager      = starter.PlayerManager;
			_levelGoalManager   = starter.LevelGoalManager;
			_pauseManager       = starter.PauseManager;
			_prefabsController  = starter.PrefabsController;
			_xpController       = starter.XpController;
			_upgradesController = starter.UpgradesController;
			_playerController   = starter.PlayerController;

			_playerHpSystem = _playerController.HpSystem;

			_actualParams = DefaultShootingParams.ShallowCopy();
			TryUpdateShootingParams();
			_defaultShootingSystem = new DefaultShootingSystem(_spawnHelper, _actualParams);

			_movementSpeed = _upgradesController.GetCurConfigMovementSpeed();

			_playerHpSystem.OnHpChanged += OnCurHpChanged;
			_playerHpSystem.OnDied      += OnDied;
			OnCurHpChanged(_playerHpSystem.Hp);

			HealthBar.Init(1f);

			if ( !PlayerDeathAnimationController.IsInit ) {
				PlayerDeathAnimationController.Init(starter);
			}
			OnRespawn();

			Physics2D.IgnoreCollision(Collider, ShieldCollider);
		}

		public void OnRespawn() {
			Rigidbody.position = _playerStartPos.position;
			Rigidbody.velocity = Vector2.zero;
			Rigidbody.rotation = 0f;

			transform.position = _playerStartPos.position;

			DeathVisualEffectRoot.SetActive(false);
			DeathVisualEffect.Stop();

			OnPlayerRespawn?.Invoke();
			PlayerDeathAnimationController.ResetAnim();
		}

		public void OnRestart() {
			Deinit();
			OnPlayerDied?.Invoke();
		}

		public void TakeDamage(float damage) {
			if ( !_playerHpSystem.IsAlive || _playerController.IsInvincible ) {
				return;
			}
			_playerController.TakeDamage(damage);
			OnPlayerTakeDamage?.Invoke();
			DamageSoundPlayer.Play();
		}

		void OnDied() {
			DeathSoundPlayer.Play();
			DeathVisualEffectRoot.SetActive(true);
			DeathVisualEffect.Play();
			UniTask.Void(OnDeath);
		}

		async UniTaskVoid OnDeath() {
			await PlayerDeathAnimationController.PlayPlayerDeathAnim();
			_levelGoalManager.OnPlayerDied();
		}

		void Deinit() {
			_playerHpSystem.OnHpChanged -= OnCurHpChanged;
			_playerHpSystem.OnDied      -= OnDied;
		}

		void OnCurHpChanged(float curHp) {
			HealthBar.Progress = (curHp / _upgradesController.GetCurConfigMaxHp());
		}

		void TryShoot() {
			TryUpdateShootingParams();
			if ( _defaultShootingSystem.TryShoot() ) {
				ShotSoundPlayer.Play();
			}
		}

		void TryUpdateShootingParams() {
			//Try update load params
			_actualParams.ReloadTime = CalcReloadTime();
			//Try update bullet
			var isX2Damage = _playerManager.HasActivePowerUp(PowerUpType.X2Damage);
			_actualParams.BulletPrefab = _prefabsController.GetBulletPrefab(isX2Damage);
			//Try update damage
			_actualParams.BulletDamage = CalcDamage();
		}

		float CalcReloadTime() {
			var fireRateMultiplier = _playerManager.HasActivePowerUp(PowerUpType.IncFireRate)
				? TmpIncFireRateMult
				: 1f;
			var fireRate = _upgradesController.GetCurConfigFireRate();
			return 1f / (fireRate * fireRateMultiplier);
		}

		float CalcDamage() {
			var damageMultiplier = _playerManager.HasActivePowerUp(PowerUpType.X2Damage) ? 2f : 1f;
			var damage           = _upgradesController.GetCurConfigDamage();
			return damageMultiplier * damage;
		}
	}
}
