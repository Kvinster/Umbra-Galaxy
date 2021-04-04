using UnityEngine;
using UnityEngine.VFX;

using System;

using STP.Behaviour.Starter;
using STP.Common;
using STP.Core;
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
		public Collider2D  Collider;
		public float       MovementSpeed;
		[Space]
		[NotNull]
		public ProgressBar HealthBar;
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

		ShootingSystem       _shootingSystem;
		ShootingSystemParams _actualParams;

		Camera            _camera;
		CoreSpawnHelper   _spawnHelper;
		Transform         _playerStartPos;
		PlayerManager     _playerManager;
		LevelGoalManager  _levelGoalManager;
		PauseManager      _pauseManager;
		PrefabsController _prefabsController;
		XpController      _xpController;

		PlayerController _playerController;

		bool IsAlive => (_playerController?.IsAlive ?? false);

		float CurHp => _playerController.CurHp;

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
			if ( !IsInit || !IsAlive ) {
				return;
			}
			if ( _pauseManager.IsPaused ) {
				return;
			}

			_input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			_shootingSystem.DeltaTick();
			if ( Input.GetMouseButton(0) ) {
				TryShoot();
			}
		}

		void FixedUpdate() {
			if ( !IsInit || !IsAlive ) {
				return;
			}
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

		void OnDestroy() {
			Deinit();
		}

		protected override void InitInternal(CoreStarter starter) {
			_camera            = starter.MainCamera;
			_spawnHelper       = starter.SpawnHelper;
			_playerStartPos    = starter.PlayerStartPos;
			_playerManager     = starter.PlayerManager;
			_levelGoalManager  = starter.LevelGoalManager;
			_pauseManager      = starter.PauseManager;
			_prefabsController = starter.PrefabsController;
			_xpController      = starter.XpController;
			_actualParams      = DefaultShootingParams.ShallowCopy();
			_shootingSystem    = new ShootingSystem(_spawnHelper, _actualParams);
			
			_playerController                  =  starter.PlayerController;
			_playerController.OnCurHpChanged   += OnCurHpChanged;
			_playerController.OnIsAliveChanged += OnIsAliveChanged;
			OnCurHpChanged(CurHp);

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
			if ( !IsAlive || _playerController.IsInvincible ) {
				return;
			}
			_playerController.TakeDamage(damage);
			OnPlayerTakeDamage?.Invoke();
			DamageSoundPlayer.Play();
		}

		void OnIsAliveChanged(bool isAlive) {
			if ( !isAlive ) {
				DeathSoundPlayer.Play();
				DeathVisualEffectRoot.SetActive(true);
				DeathVisualEffect.Play();

				UniTask.Void(OnDeath);
			}
		}

		async UniTaskVoid OnDeath() {
			await PlayerDeathAnimationController.PlayPlayerDeathAnim();
			_levelGoalManager.OnPlayerDied();
		}

		void Deinit() {
			_playerController.OnCurHpChanged   -= OnCurHpChanged;
			_playerController.OnIsAliveChanged -= OnIsAliveChanged;
		}

		void OnCurHpChanged(float curHp) {
			HealthBar.Progress = (curHp / PlayerController.MaxPlayerHp);
		}

		void TryShoot() {
			TryUpdateShootingParams();
			if ( _shootingSystem.TryShoot() ) {
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
			var firerateMultiplier = _playerManager.HasActivePowerUp(PowerUpType.IncFireRate)
				? TmpIncFireRateMult
				: 1f;
			return Mathf.Max((DefaultShootingParams.ReloadTime - 0.01f * _xpController.Level), DefaultShootingParams.ReloadTime / 10f)  / firerateMultiplier;
		}
		
		float CalcDamage() {
			var damageMultiplier     = _playerManager.HasActivePowerUp(PowerUpType.X2Damage) ? 2f : 1f;
			var levelDependentDamage = DefaultShootingParams.BulletDamage + 1.5f * _xpController.Level;
			return damageMultiplier * levelDependentDamage;
		}
	}
}
