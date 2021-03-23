using UnityEngine;

using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Starter {
	public sealed class EndlessLevelStarter : BaseStarter<EndlessLevelStarter> {
		[NotNull] public Camera Camera;

		void Start() {
			InitComponents();
		}
	}
}
