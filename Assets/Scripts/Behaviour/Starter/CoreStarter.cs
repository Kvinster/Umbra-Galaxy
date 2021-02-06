using UnityEngine;

using System.Collections;

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
		[NotNull] public Camera                    MainCamera;
		[NotNull] public Player                    Player;
		[NotNull] public Camera                    MinimapCamera;
		[NotNull] public Transform                 PlayerStartPos;
		[NotNull] public LevelGenerator            Generator;
		[NotNull] public CoreWindowsManager        CoreWindowsManager;
		[NotNull] public SceneTransitionController SceneTransitionController;

		[NotNull] public Transform LevelObjectsRoot;
		[NotNull] public Transform TempObjectsRoot;

		bool _isLevelInitStarted;

		public CoreSpawnHelper   SpawnHelper       { get; private set; }
		public PauseManager      PauseManager      { get; private set; }
		public LevelManager      LevelManager      { get; private set; }
		public PlayerManager     PlayerManager     { get; private set; }
		public LevelGoalManager  LevelGoalManager  { get; private set; }
		public MinimapManager    MinimapManager    { get; private set; }
		public GameController    GameController    { get; private set; }
		public ProfileController ProfileController { get; private set; }
		public PlayerController  PlayerController  => ProfileController.PlayerController;
		public XpController      XpController      => ProfileController.XpController;

		void OnDisable() {
			GameController.Deinit();
			LevelGoalManager.Deinit();
		}

		IEnumerator Start() {
			yield return null;
			if ( !_isLevelInitStarted ) { // to properly initialize when started from editor
				StartCoroutine(InitLevel());
			}
		}

		public IEnumerator InitLevel() {
			_isLevelInitStarted = true;
#if UNITY_EDITOR
			if ( !GameState.IsActiveInstanceExists ) {
				Debug.Log("Trying to load GameState instance");
				var gs = GameState.TryLoadActiveGameState();
				if ( gs == null ) {
					Debug.Log("Loading failed, creating new GameState instance");
					GameState.CreateNewActiveGameState();
				}
			}
			if ( !ProfileState.IsActiveInstanceExists ) {
				Debug.Log("Creating new ProfileState instance");
				var gs = ProfileState.CreateNewActiveGameState("test", "test");
				gs.LevelState.CurLevelIndex = 0;
			}
			if ( !ProfileController.IsActiveInstanceExists ) {
				ProfileController.CreateNewActiveInstance(ProfileState.ActiveInstance);
			}
#endif
			GameController    = new GameController(GameState.ActiveInstance);
			ProfileController = ProfileController.ActiveInstance;
			var pc  = ProfileController.PlayerController;
			var lc  = ProfileController.LevelController;
			var xc  = ProfileController.XpController;
			var puc = ProfileController.PowerUpController;
			SpawnHelper   = new CoreSpawnHelper(this, TempObjectsRoot);
			PauseManager  = new PauseManager();
			LevelManager  = new LevelManager(Player.transform, SceneTransitionController, PauseManager, lc);
			PlayerManager = new PlayerManager(Player, pc, xc, UnityContext.Instance, TempObjectsRoot);
			LevelGoalManager = new LevelGoalManager(PlayerManager, LevelManager, pc, lc, xc,
				GameController.LeaderboardController, ProfileState.ActiveInstance);
			CoreWindowsManager.Init(PauseManager, LevelManager, LevelGoalManager, PlayerManager, pc, xc);
			MinimapManager = new MinimapManager(MinimapCamera);
			Generator.Init(puc, lc);
			yield return Generator.GenerateLevel(LevelObjectsRoot);
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
