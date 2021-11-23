using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Core.Enemy;
using STP.Behaviour.Core.Enemy.BossSpawner;
using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

using Shapes;

namespace STP.Behaviour.Core {
	public sealed class LevelExplosionZone : BaseCoreComponent {
		static readonly HashSet<Type> MainEnemyTypes = new HashSet<Type> {
			typeof(Generator),
			typeof(SpawnerBossController),
			typeof(SpawnerBossController)
		};

		[Header("Parameters")]
		public float StartRadius;
		public float MaxRadius;
		public float GrowSpeed;
		public float RadiusTolerance = 10f;
		[Header("Dependencies")]
		[NotNull] public CircleCollider2D Collider;

		public Disc Disc;

		bool _stoppedGrow;

		public bool IgnoreMainEnemies { get; set; }
		public bool UseUnscaledTime   { get; set; }

		void Update() {
			if ( _stoppedGrow || !UseUnscaledTime ) {
				return;
			}
			var radius = Collider.radius + Time.unscaledDeltaTime * GrowSpeed;
			Collider.radius = radius;
			if ( Disc ) {
				Disc.Radius = radius;
			}
			if ( radius >= MaxRadius ) {
				_stoppedGrow = true;
			}
		}

		void FixedUpdate() {
			if ( _stoppedGrow || UseUnscaledTime ) {
				return;
			}
			var radius = Collider.radius + Time.fixedDeltaTime * GrowSpeed;
			Collider.radius = radius;
			if ( Disc ) {
				Disc.Radius = radius;
			}
			if ( radius >= MaxRadius ) {
				_stoppedGrow = true;
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			ResetToStart();
		}

		public void ResetToStart() {
			Collider.radius = StartRadius;
			if ( Disc ) {
				Disc.Radius = StartRadius;
			}
		}

		public void SetActive(bool isActive) {
			gameObject.SetActive(isActive);
		}

		void OnTriggerEnter2D(Collider2D other) {
			var distance = Vector2.Distance(transform.position, other.transform.position);
			if ( (distance < Collider.radius) && (Mathf.Abs(distance - Collider.radius) > RadiusTolerance) ) {
				return;
			}
			if ( other.TryGetComponent<BaseEnemy>(out var enemy) ) {
				var ignore = IgnoreMainEnemies && MainEnemyTypes.Contains(enemy.GetType());
				if ( !ignore && enemy.IsAlive ) {
					enemy.Die(false);
				}
			}
			if ( other.TryGetComponent<IBullet>(out var bullet) ) {
				bullet.Die();
			}
			if ( other.TryGetComponent<Asteroid>(out var asteroid) ) {
				asteroid.Die(false);
			}
		}
	}
}
