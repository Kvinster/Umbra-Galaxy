using UnityEngine;

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
using STP.Service;

namespace STP.Behaviour.Core {
	public sealed class Player : BaseCoreComponent, IDestructible {
		const float TmpIncFireRateMult = 4f;
		const float TripleShotAngle    = 15f;

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
		public PlayerDeathAnimationController PlayerDeathAnimationController;

		[BoxGroup("Sound")] [NotNull] public BaseSimpleSoundPlayer DamageSoundPlayer;
		[BoxGroup("Sound")] [NotNull] public BaseSimpleSoundPlayer ShotSoundPlayer;
		[BoxGroup("Sound")] [NotNull] public BaseSimpleSoundPlayer TripleShotSoundPlayer;

		Vector2 _input;

		DefaultShootingSystem    _defaultShootingSystem;
		TripleShotShootingSystem _tripleShotShootingSystem;
		ShootingSystemParams     _actualParams;

		Camera            _camera;
		CoreSpawnHelper   _spawnHelper;
		Transform         _playerStartPos;
		PlayerManager     _playerManager;
		LevelGoalManager  _levelGoalManager;
		LevelManager      _levelManager;
		PauseManager      _pauseManager;
		PrefabsController _prefabsController;
		PlayerController  _playerController;

		float _movementSpeed;
		bool  _movementDisabled;

		HpSystem _playerHpSystem;

		bool CanMove => IsInit && _playerHpSystem.IsAlive && !_pauseManager.IsPaused && _levelManager.IsLevelActive &&
		                !_movementDisabled;

		BaseShootingSystem CurShootingSystem { get; set; }

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

		void OnDestroy() {
			Deinit();
		}

		void Update() {
			if ( !CanMove ) {
				return;
			}

			_input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			CurShootingSystem.DeltaTick();
			if ( Input.GetMouseButton(0) ) {
				TryShoot();
			}
		}

		void FixedUpdate() {
			if ( !CanMove ) {
				return;
			}
			if ( _input != Vector2.zero ) {
				Rigidbody.AddForce(_input.normalized * _movementSpeed, ForceMode2D.Impulse);
			}
			var mouseWorldPos  = _camera.ScreenToWorldPoint(Input.mousePosition);
			var neededRotation = -Vector2.SignedAngle(mouseWorldPos - transform.position, Vector2.up);
			Rigidbody.MoveRotation(neededRotation);
		}

		protected override void InitInternal(CoreStarter starter) {
			_camera            = starter.MainCamera;
			_spawnHelper       = starter.SpawnHelper;
			_playerStartPos    = starter.PlayerStartPos;
			_playerManager     = starter.PlayerManager;
			_levelGoalManager  = starter.LevelGoalManager;
			_levelManager      = starter.LevelManager;
			_pauseManager      = starter.PauseManager;
			_prefabsController = starter.PrefabsController;
			_playerController  = starter.PlayerController;

			Rigidbody.centerOfMass = Vector2.zero;

			_playerHpSystem = _playerController.HpSystem;

			_actualParams = DefaultShootingParams.ShallowCopy();
			TryUpdateShootingParams();
			_defaultShootingSystem    = new DefaultShootingSystem(_spawnHelper, _actualParams);
			_tripleShotShootingSystem = new TripleShotShootingSystem(TripleShotAngle, _spawnHelper, _actualParams);

			CurShootingSystem = _defaultShootingSystem;

			_movementSpeed = _playerController.Config.MovementSpeed;

			_playerHpSystem.OnHpChanged += OnCurHpChanged;
			_playerHpSystem.OnDied      += OnDied;
			OnCurHpChanged(_playerHpSystem.Hp);

			HealthBar.Init(1f);

			if ( !PlayerDeathAnimationController.IsInit ) {
				PlayerDeathAnimationController.Init(starter);
			}
			OnRespawn();

			_playerManager.OnPowerUpStarted  += OnPowerUpStarted;
			_playerManager.OnPowerUpFinished += OnPowerUpFinished;

			Physics2D.IgnoreCollision(Collider, ShieldCollider);
		}

		public void OnRespawn() {
			Rigidbody.position = _playerStartPos.position;
			Rigidbody.velocity = Vector2.zero;
			Rigidbody.rotation = 0f;

			transform.position = _playerStartPos.position;

			OnPlayerRespawn?.Invoke();
			PlayerDeathAnimationController.ResetAnim();
		}

		public void OnRestart() {
			Deinit();
			OnPlayerDied?.Invoke();
		}

		public void TakeDamage(float damage) {
			if ( !_playerHpSystem.IsAlive || _playerController.IsInvincible || _levelGoalManager.IsLevelWon ) {
				return;
			}
			_playerController.TakeDamage(damage);
			OnPlayerTakeDamage?.Invoke();
			DamageSoundPlayer.Play();
		}

		public void DisableMovement() {
			_movementDisabled = true;
		}

		void OnDied() {
			_playerController.SubLife();
			if ( _playerController.CurLives == 0 ) {
				OnFinalDeath();
				AnalyticsService.LogEvent(new PlayerDiedEvent());
			} else {
				OnDeath();
				AnalyticsService.LogEvent(new PlayerLostLifeEvent());
			}
		}

		void OnDeath() {
			PlayerDeathAnimationController.PlayPlayerDeathAnim().Forget();
		}

		void OnFinalDeath() {
			PlayerDeathAnimationController.PlayFinalPlayerDeathAnim().Forget();
		}

		void Deinit() {
			_playerHpSystem.OnHpChanged -= OnCurHpChanged;
			_playerHpSystem.OnDied      -= OnDied;
			if ( _playerManager != null ) {
				_playerManager.OnPowerUpStarted  -= OnPowerUpStarted;
				_playerManager.OnPowerUpFinished -= OnPowerUpFinished;
			}
		}

		void OnCurHpChanged(float curHp) {
			HealthBar.Progress = (curHp / _playerController.Config.MaxHp);
		}

		void TryShoot() {
			TryUpdateShootingParams();
			if ( CurShootingSystem.TryShoot() ) {
				if ( _playerManager.HasActivePowerUp(PowerUpType.TripleShot) ) {
					TripleShotSoundPlayer.Play();
				} else {
					ShotSoundPlayer.Play();
				}
			}
		}

		void OnPowerUpStarted(PowerUpType powerUpType) {
			switch ( powerUpType ) {
				case PowerUpType.TripleShot: {
					CurShootingSystem = _tripleShotShootingSystem;
					break;
				}
				default: {
					break;
				}
			}
		}

		void OnPowerUpFinished(PowerUpType powerUpType) {
			switch ( powerUpType ) {
				case PowerUpType.TripleShot: {
					CurShootingSystem = _defaultShootingSystem;
					break;
				}
				default: {
					break;
				}
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
			return 1f / _playerController.Config.FireRate;
		}

		float CalcDamage() {
			var damageMultiplier = _playerManager.HasActivePowerUp(PowerUpType.X2Damage) ? 2f : 1f;
			var damage           = _playerController.Config.Damage;
			return damageMultiplier * damage;
		}
	}
}
