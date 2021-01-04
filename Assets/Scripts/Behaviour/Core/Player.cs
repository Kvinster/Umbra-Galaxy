using UnityEngine;

using System;

using STP.Behaviour.Starter;
using STP.Controller;
using STP.Manager;

namespace STP.Behaviour.Core {
	public sealed class Player : BaseCoreComponent, IDestructible {
		public Rigidbody2D Rigidbody;
		public Collider2D  Collider;
		public float       MovementSpeed;
		[Space]
		public GameObject BulletPrefab;
		public float BulletStartForce;
		[Space]
		public float ReloadDuration;
		[Space]
		public ProgressBar HealthBar;

		Vector2 _input;

		Camera           _camera;
		Transform        _playerStartPos;
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
			_playerStartPos   = starter.PlayerStartPos;
			_levelGoalManager = starter.LevelGoalManager;

			_playerController                =  PlayerController.Instance;
			_playerController.OnCurHpChanged += OnCurHpChanged;
			OnCurHpChanged(CurHp);

			HealthBar.Init(1f);

			Respawn();
		}

		public void TakeDamage(float damage) {
			if ( _playerController.TakeDamage(damage) ) {
				if ( _playerController.TrySubLives() ) {
					Respawn();
				} else {
					Die();
				}
			}
		}

		void Deinit() {
			_playerController.OnCurHpChanged -= OnCurHpChanged;
		}

		void Respawn() {
			_playerController.RestoreHp();
			Rigidbody.position = _playerStartPos.position;
			Rigidbody.velocity = Vector2.zero;
			Rigidbody.rotation = 0f;
			_reloadTimer       = 0f;
		}

		void Die() {
			Deinit();
			_playerController.RestoreHp();
			_playerController.RestoreLives();
			OnPlayerDied?.Invoke();
			_levelGoalManager.LoseLevel();
		}

		void OnCurHpChanged(float curHp) {
			HealthBar.Progress = (curHp / PlayerController.MaxPlayerHp);
		}

		void TryShoot() {
			if ( _reloadTimer <= 0 ) {
				Shoot();
				_reloadTimer = ReloadDuration;
			}
		}

		void Shoot() {
			var bulletGo = Instantiate(BulletPrefab, transform.position, Quaternion.identity, null);
			var bulletRb = bulletGo.GetComponent<Rigidbody2D>();
			bulletRb.rotation = Rigidbody.rotation;
			bulletRb.AddRelativeForce(Vector2.up * BulletStartForce, ForceMode2D.Impulse);
			Physics2D.IgnoreCollision(Collider, bulletGo.GetComponent<Collider2D>());
		}
	}
}
