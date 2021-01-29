using UnityEngine;

using STP.Behaviour.Core;
using STP.Behaviour.Core.Generators;
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

		[NotNull] public Transform LevelObjectsRoot;
		[NotNull] public Transform TempObjectsRoot;

		public CoreSpawnHelper  SpawnHelper      { get; private set; }
		public PauseManager     PauseManager     { get; private set; }
		public LevelManager     LevelManager     { get; private set; }
		public PlayerManager    PlayerManager    { get; private set; }
		public LevelGoalManager LevelGoalManager { get; private set; }
		public MinimapManager   MinimapManager   { get; private set; }

		public LeaderboardController LeaderboardController { get; private set; }
		public GameController        GameController        { get; private set; }
		public PlayerController      PlayerController      => GameController.PlayerController;
		public XpController          XpController          => GameController.XpController;

		void OnDisable() {
			GameController.Deinit();
		}

		void Start() {
#if UNITY_EDITOR
			if ( !GameState.IsActiveInstanceExists ) {
				Debug.Log("Creating new GameState instance");
				var gs = GameState.CreateNewActiveGameState("test", System.Guid.NewGuid().ToString());
				gs.LevelState.CurLevelIndex = 0;
			}
#endif
			LeaderboardController = new LeaderboardController();
			GameController        = new GameController(GameState.ActiveInstance);
			var pc  = GameController.PlayerController;
			var lc  = GameController.LevelController;
			var xc  = GameController.XpController;
			var cc  = GameController.ChunkController;
			var puc = GameController.PowerUpController;
			SpawnHelper   = new CoreSpawnHelper(this, TempObjectsRoot);
			PauseManager  = new PauseManager();
			LevelManager  = new LevelManager(Player.transform, PauseManager, lc);
			PlayerManager = new PlayerManager(Player, pc, xc, UnityContext.Instance, TempObjectsRoot);
			CoreWindowsManager.Init(PauseManager, LevelManager, PlayerManager, pc, xc);
			LevelGoalManager = new LevelGoalManager(PlayerManager, LevelManager, CoreWindowsManager, lc, xc,
				LeaderboardController, GameState.ActiveInstance);
			MinimapManager   = new MinimapManager(MinimapCamera);
			Generator.Init(cc, puc);
			Generator.GenerateLevel(lc.GetCurLevelConfig(), cc.GetChunkPrefab, LevelObjectsRoot);
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
