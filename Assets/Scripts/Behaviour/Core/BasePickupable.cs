using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public abstract class BasePickupable : BaseCoreComponent {
		[NotNull] public TriggerNotifier Notifier;

		protected void Reset() {
			Notifier = GetComponentInChildren<TriggerNotifier>();
		}

		protected override void InitInternal(CoreStarter starter) {
			Notifier.OnTriggerEnter += OnRangeEnter;
			Notifier.OnTriggerStay  += OnRangeStay;
		}

		protected void OnDestroy() {
			Notifier.OnTriggerEnter -= OnRangeEnter;
			Notifier.OnTriggerStay  -= OnRangeStay;
		}

		protected virtual void OnRangeEnter(GameObject go) {
			OnObjectTouch(go);
		}

		protected virtual void OnRangeStay(GameObject go) {
			OnObjectTouch(go);
		}

		void OnObjectTouch(GameObject go) {
			var playerComp = go.GetComponent<Player>();
			if ( !playerComp ) {
				return;
			}
			if ( OnPlayerEnter() ) {
				Destroy(gameObject);
			}
		}

		protected abstract bool OnPlayerEnter();
	}
}
