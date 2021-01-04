using UnityEngine;

using STP.Behaviour.Core;
using STP.Controller;
using STP.Manager;
using STP.Utils.GameComponentAttributes;
using STP.View.DebugGUI;

namespace STP.Behaviour.Starter {
	public class CoreStarter : BaseStarter<CoreStarter> {
		[NotNull] public Player         Player;
		[NotNull] public Transform      PlayerStartPos;
		[NotNull] public LevelGenerator Generator;

		public LevelGoalManager LevelGoalManager { get; private set; }

		void Start() {
			LevelGoalManager = new LevelGoalManager(Player.transform);
			Generator.GenerateLevel(LevelController.Instance);
			InitComponents();
			// Settings for smooth gameplay
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
			QualitySettings.vSyncCount  = 0;
		}

		void OnDestroy() {
			if ( DebugGuiController.HasInstance ) {
				DebugGuiController.Instance.SetDrawable(null);
			}
		}
	}
}