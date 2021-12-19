using STP.Core;

namespace STP.Behaviour.Core.Enemy {
	public abstract class BaseBoss : BaseEnemy, IHpSource, IDestructible{
		public abstract HpSystem HpSystemComponent { get; }

		public abstract void TakeDamage(float damage);
	}
}