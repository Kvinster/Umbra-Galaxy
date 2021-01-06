using STP.Events;
using STP.Utils.Events;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public abstract class BaseEnemy : BaseCoreComponent {
		[NotNullOrEmpty] public string Name;

		protected virtual void Die() {
			EventManager.Fire(new EnemyDestroyed(Name));
		}
	}
}