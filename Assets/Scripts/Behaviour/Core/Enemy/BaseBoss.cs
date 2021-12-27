using UnityEngine;

using STP.Core;
using STP.Utils.GameComponentAttributes;

using NaughtyAttributes;

namespace STP.Behaviour.Core.Enemy {
	public abstract class BaseBoss : BaseEnemy, IHpSource, IDestructible {
		[BoxGroup("Portal")] [NotNull] public Portal    Portal;
		[BoxGroup("Portal")] [NotNull] public Transform PortalAppearPosition;

		public abstract HpSystem HpSystemComponent { get; }

		public abstract void TakeDamage(float damage);

		public void PrepareAppear() {
			Portal.PrepareTargetAppearAnim(transform, PortalAppearPosition);
		}

		public void Appear() {
			Portal.PlayTargetAppearAnim(transform, PortalAppearPosition, Activate);
		}

		protected abstract void Activate();
	}
}
