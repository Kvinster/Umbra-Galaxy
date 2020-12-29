﻿using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public sealed class Fighter : BaseStarterCoreComponent, IDestructible {
		public float StartHp;
		public float MovementSpeed;
		[Range(0f, 1f)]
		public float RotationSpeed;
		[NotNull]
		public Rigidbody2D Rigidbody;
		[NotNull] 
		public Collider2D  Collider;
		[NotNull]
		public TriggerNotifier DetectRangeNotifier;

		[Header("Gun")] 
		[NotNull] 
		public GameObject Bullet;
		public float      FirePeriod;
		public float      StartBulletForce;

		readonly Timer _fireTimer = new Timer();

		Transform _target;

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
			if ( !_target ) {
				return;
			}
			Rigidbody.MovePosition(transform.position + transform.up * (MovementSpeed * Time.fixedDeltaTime));
		}

		protected override void InitInternal(CoreStarter starter) {
			CurHp = StartHp;
			_fireTimer.Start(FirePeriod);
			DetectRangeNotifier.OnTriggerEnter += OnDetectRangeEnter;
			DetectRangeNotifier.OnTriggerExit  += OnDetectRangeExit;
		}

		public void TakeDamage(float damage) {
			CurHp = Mathf.Max(CurHp - damage, 0);
			if ( CurHp == 0 ) {
				Die();
			}
		}

		void Die() {
			DetectRangeNotifier.OnTriggerEnter -= OnDetectRangeEnter;
			DetectRangeNotifier.OnTriggerExit  -= OnDetectRangeExit;

			Destroy(gameObject);
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
			var destructible = other.gameObject.GetComponent<IDestructible>();
			if ( destructible != null ) {
				destructible.TakeDamage(20);
				Die();
			}
		}

		void Fire() {
			var go = Instantiate(Bullet, transform.position, Quaternion.identity);
			var bullet = go.GetComponent<Bullet>();
			if ( !bullet ) {
				Debug.LogError("Can't init bullet in fighter. Component Bullet not found");
				Destroy(go);
				return;
			}
			bullet.Init(Collider, Vector2.up * StartBulletForce, transform.rotation.eulerAngles.z);
		}
	}
}