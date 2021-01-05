using STP.Events;
using STP.Utils.Events;

namespace STP.Behaviour.Core.Enemy {
	public abstract class BaseEnemy : BaseCoreComponent {
		public string Name;

		protected virtual void Die() {
			EventManager.Fire(new EnemyDestroyed(Name));
		}
	}
}