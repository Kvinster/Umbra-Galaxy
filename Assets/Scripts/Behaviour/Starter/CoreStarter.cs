using UnityEngine;

using STP.Behaviour.Core;
using STP.Core;
using STP.Core.State;
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
		public LevelManager     LevelManager     { get; private set; }
		public PlayerManager    PlayerManager    { get; private set; }
		public LevelGoalManager LevelGoalManager { get; private set; }
		public MinimapManager   MinimapManager   { get; private set; }

		public GameController   GameController   { get; private set; }
		public PlayerController PlayerController => GameController.PlayerController;
		public XpController     XpController     => GameController.XpController;

		void OnDisable() {
			GameController.Deinit();
		}

		void Start() {
#if UNITY_EDITOR
			if ( !GameState.IsActiveInstanceExists ) {
				Debug.Log("Creating new GameState instance");
				GameState.CreateNewActiveGameState("test");
			}
#endif
			GameController = new GameController(GameState.ActiveInstance);
			var pc = GameController.PlayerController;
			var lc = GameController.LevelController;
			var xc = GameController.XpController;
			var cc = GameController.ChunkController;
			SpawnHelper      = new CoreSpawnHelper(this);
			PauseManager     = new PauseManager();
			LevelManager     = new LevelManager(Player.transform, PauseManager);
			PlayerManager    = new PlayerManager(Player, pc, xc, UnityContext.Instance);
			CoreWindowsManager.Init(PauseManager, LevelManager, PlayerManager, pc, xc);
			LevelGoalManager = new LevelGoalManager(LevelManager, CoreWindowsManager, lc, xc);
			MinimapManager   = new MinimapManager(MinimapCamera);
			Generator.Init(lc, cc);
			Generator.GenerateLevel(lc.GetCurLevelConfig(), cc.GetChunkPrefab);
			InitComponents();
			// Settings for smooth gameplay
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
			QualitySettings.vSyncCount  = 0;
			xc.OnLevelStart();
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
