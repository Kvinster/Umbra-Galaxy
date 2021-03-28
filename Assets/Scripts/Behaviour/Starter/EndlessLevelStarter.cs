using UnityEngine;

using STP.Behaviour.EndlessLevel;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Starter {
	public sealed class EndlessLevelStarter : BaseStarter<EndlessLevelStarter> {
		[NotNull] public Camera             Camera;
		[NotNull] public EndlessLevelPlayer Player;
		[NotNull] public Transform          TmpObjectsRoot;
		[NotNull] public WaveController     WaveController;
		
		void Start() {
			InitComponents();
		}
	}
}
