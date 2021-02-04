using System;

using STP.Utils;

using RSG;

namespace STP.Behaviour.Core.UI {
	public abstract class BaseCoreWindow : GameComponent {
		protected Promise ShowPromise;

		public bool IsShown => (ShowPromise != null);

		public virtual IPromise Show() {
			if ( IsShown ) {
				return Promise.Rejected(new Exception($"{GetType().Name} is already shown"));
			}

			gameObject.SetActive(true);

			ShowPromise = new Promise();
			return ShowPromise;
		}

		protected virtual void Hide() {
			gameObject.SetActive(false);
			ShowPromise.Resolve();
			ShowPromise = null;
		}
	}
}
