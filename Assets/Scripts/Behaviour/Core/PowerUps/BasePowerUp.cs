using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.PowerUps {
	public abstract class BasePowerUp : BaseCoreComponent {
		[NotNull] 
		public TriggerNotifier Notifier;
		
		protected override void InitInternal(CoreStarter starter) {
			Notifier.OnTriggerEnter += OnRangeEnter;
		}

		void OnDestroy() {
			Notifier.OnTriggerEnter -= OnRangeEnter;
		}

		protected abstract void OnRangeEnter(GameObject go);
	}
}