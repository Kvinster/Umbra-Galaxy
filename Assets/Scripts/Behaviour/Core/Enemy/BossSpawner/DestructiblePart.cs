using System;
using STP.Core;
using STP.Utils;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class DestructiblePart : GameComponent, IDestructible {
		public float StartHp;

		HpSystem _hpSystem;

		public event Action<DestructiblePart> OnDiedEvent;
		
		protected void InitInternal() {
			_hpSystem        =  new HpSystem(StartHp);
			_hpSystem.OnDied += OnDied;
		}
 
		public void TakeDamage(float damage) {
			_hpSystem.TakeDamage(damage);
		}

		void OnDied() {
			OnDiedEvent?.Invoke(this);
			Destroy(gameObject);
		}
	}
}