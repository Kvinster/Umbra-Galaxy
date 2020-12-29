using UnityEngine;

using System;

using STP.Behaviour.Starter;

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
		public int StartHp = 100;
		public ProgressBar HealthBar;

		Vector2 _input;

		Camera _camera;

		float _reloadTimer;

		float _curHp;

		float CurHp {
			get => _curHp;
			set {
				_curHp             = value;
				HealthBar.Progress = _curHp / StartHp;
			}
		}

		public event Action OnPlayerDied;

		void Reset() {
			Rigidbody = GetComponent<Rigidbody2D>();
		}

		protected override void InitInternal(CoreStarter starter) {
			_camera = Camera.main;

			CurHp = StartHp;

			HealthBar.Init(1f);
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

		public void TakeDamage(float damage) {
			CurHp = Mathf.Max(CurHp - damage, 0);
			if ( CurHp == 0 ) {
				Die();
			}
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

		void Die() {
			OnPlayerDied?.Invoke();
			Destroy(gameObject);
		}
	}
}
