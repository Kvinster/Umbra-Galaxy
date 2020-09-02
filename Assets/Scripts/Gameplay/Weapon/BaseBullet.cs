using UnityEngine;

using System;

using STP.Utils;

namespace STP.Gameplay.Weapon {
    public abstract class BaseBullet : GameComponent {
        public event Action OnDestroy;

        readonly Timer _timer = new Timer();

        protected void InitTimer(float flightTime) {
            _timer.Start(flightTime);
        }

        void Update() {
            if ( _timer.Tick(Time.deltaTime) ) {
                OnDestroy?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}