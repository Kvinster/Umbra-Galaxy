using UnityEngine;

using STP.Behaviour.Core;
using STP.Core;
using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;
using STP.View.DebugGUI;

namespace STP.Behaviour.Starter {
	public class CoreStarter : BaseStarter<CoreStarter> {
		[NotNull] public Player             Player;
		[NotNull] public Camera             MinimapCamera;
		[NotNull] public Transform          PlayerStartPos;
		[NotNull] public LevelGenerator     Generator;
		[NotNull] public CoreWindowsManager CoreWindowsManager;

		public CoreSpawnHelper  SpawnHelper      { get; private set; }
		public PauseManager     PauseManager     { get; private set; }
		public PlayerManager    PlayerManager    { get; private set; }
		public LevelGoalManager LevelGoalManager { get; private set; }
		public MinimapManager   MinimapManager   { get; private set; }

		public PlayerController PlayerController => GameState.Instance.PlayerController;
		public XpController     XpController     => GameState.Instance.XpController;

		void Start() {
#if UNITY_EDITOR
			if ( !GameState.IsInstanceExists ) {
				Debug.Log("Creating new GameState instance");
				GameState.CreateNewGameState();
			}
#endif
			SpawnHelper  = new CoreSpawnHelper(this);
			PauseManager = new PauseManager();
			var gc = GameState.Instance;
			var pc = gc.PlayerController;
			var lc = gc.LevelController;
			var xc = gc.XpController;
			var cc = gc.ChunkController;
			PlayerManager    = new PlayerManager(Player, pc, xc, UnityContext.Instance);
			LevelGoalManager = new LevelGoalManager(Player.transform, PauseManager, lc);
			MinimapManager   = new MinimapManager(MinimapCamera);
			CoreWindowsManager.Init(PauseManager, PlayerManager, LevelGoalManager, pc, xc);
			Generator.Init(lc, cc);
			Generator.GenerateLevel(lc.GetCurLevelConfig(), cc.GetChunkPrefab);
			InitComponents();
			// Settings for smooth gameplay
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
			QualitySettings.vSyncCount  = 0;
		}

		void OnDestroy() {
			PauseManager?.Deinit();
			if ( DebugGuiController.HasInstance ) {
				DebugGuiController.Instance.SetDrawable(null);
			}
			PlayerManager?.Deinit();
		}
	}
}
