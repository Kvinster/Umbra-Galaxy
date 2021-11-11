﻿using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
    public class Asteroid : BaseEnemy, IDestructible {
        public float LifeTime         = 10f;
        public float RotationVelocity = 500;

        [NotNull]
        public Rigidbody2D Rigidbody;

        readonly Timer _lifeTimer = new Timer();

        void OnCollisionEnter2D(Collision2D other) {
            other.TryTakeDamage(float.MaxValue);
        }

        void Update() {
            if ( _lifeTimer.DeltaTick() ) {
                Destroy(gameObject);
            }
        }

        protected override void InitInternal(CoreStarter starter) {
            base.InitInternal(starter);
            HpSystem.OnDied += Die;
        }

        public void SetParams(Vector2 direction, float speed) {
            Rigidbody.centerOfMass = Vector2.zero;
            Rigidbody.AddRelativeForce(speed * Rigidbody.mass * direction, ForceMode2D.Impulse);
            Rigidbody.AddTorque(RotationVelocity * Rigidbody.mass * 2 * Mathf.PI, ForceMode2D.Impulse);
            _lifeTimer.Start(LifeTime);
        }

        public override void OnBecomeVisibleForPlayer(Transform playerTransform) {
            // Do nothing
        }

        public override void OnBecomeInvisibleForPlayer() {
            // Do nothing
        }

        public override void SetTarget(Transform target) {
            // Do nothing
        }

        public void TakeDamage(float damage) {
            HpSystem.TakeDamage(damage);
        }

        void Die() {
            Die(true);
            Destroy(gameObject);
        }
    }
}