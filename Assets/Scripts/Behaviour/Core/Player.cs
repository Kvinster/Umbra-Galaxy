using System;

using UnityEngine;

using STP.Behaviour.Starter;

namespace STP.Behaviour.Core {
	public sealed class Player : BaseStarterCoreComponent {
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

		int _curHp;

		int CurHp {
			get => _curHp;
			set {
				_curHp             = value;
				HealthBar.Progress = (float) _curHp / StartHp;
			}
		}

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

		void TakeDamage(int damage) {
			CurHp = Mathf.Clamp(CurHp - damage, 0, StartHp);
			if ( CurHp == 0 ) {
				Die();
			}
		}

		void Die() {
			Destroy(gameObject);
		}

		void OnCollisionEnter2D(Collision2D other) {
			if ( other.gameObject.GetComponent<Bullet>() ) {
				TakeDamage(10);
			}
		}
	}
}
