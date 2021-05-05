using UnityEngine;

using System.Collections;

using STP.Behaviour.Core;
using STP.Behaviour.Core.Generators;
using STP.Behaviour.Utils;
using STP.Core;
using STP.Core.State;
using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;
using STP.View.DebugGUI;

using Cysharp.Threading.Tasks;

namespace STP.Behaviour.Starter {
	public class CoreStarter : BaseStarter<CoreStarter> {
		[NotNull] public Camera                      MainCamera;
		[NotNull] public RestrictedTransformFollower PlayerCameraFollower;
		[NotNull] public Camera                      MinimapCamera;
		[NotNull] public Transform                   PlayerStartPos;
		[NotNull] public CoreWindowsManager          CoreWindowsManager;
		[NotNull] public SceneTransitionController   SceneTransitionController;

		[NotNull] public Transform LevelObjectsRoot;
		[NotNull] public Transform TempObjectsRoot;
		[NotNull] public Transform BordersRoot;

		bool _isLevelInitStarted;

		public Player            Player            { get; set; }
		public CoreSpawnHelper   SpawnHelper       { get; private set; }
		public PauseManager      PauseManager      { get; private set; }
		public LevelManager      LevelManager      { get; private set; }
		public PlayerManager     PlayerManager     { get; private set; }
		public LevelGoalManager  LevelGoalManager  { get; private set; }
		public MinimapManager    MinimapManager    { get; private set; }
		public GameController    GameController    { get; private set; }
		public ProfileController ProfileController { get; private set; }
		public ShipCreator       ShipCreator       { get; private set; }

		public PlayerController  PlayerController  => ProfileController.PlayerController;
		public XpController      XpController      => ProfileController.XpController;
		public PrefabsController PrefabsController => ProfileController.PrefabsController;
		public LevelController   LevelController   => ProfileController.LevelController;

		void OnDisable() {
			GameController.Deinit();
			LevelGoalManager.Deinit();
		}

		IEnumerator Start() {
			yield return null;
			if ( !_isLevelInitStarted ) { // to properly initialize when started from editor
				#pragma warning disable 4014
				InitLevel();
				#pragma warning restore 4014
			}
		}

		public async UniTask InitLevel() {
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
			if ( !ProfileController.IsActiveInstanceExists ) {
				Debug.Log("Creating new ProfileState instance");
				var ps         = ProfileState.CreateNewProfileState("test", "test");
				var controller = ProfileController.CreateNewActiveInstance(ps);
				controller.StartLevel(0);
			}
#endif
			GameController    = new GameController(GameState.ActiveInstance);
			ProfileController = ProfileController.ActiveInstance;
			ShipCreator       = new ShipCreator(LevelObjectsRoot, ProfileController.PrefabsController);
			var pc = ProfileController.PlayerController;
			var lc = ProfileController.LevelController;
			var xc = ProfileController.XpController;
			SpawnHelper   = new CoreSpawnHelper(this, TempObjectsRoot);
			PauseManager  = new PauseManager();
			Player        = ShipCreator.CreatePlayerShip(PlayerController.Ship);
			LevelManager  = new LevelManager(Player.transform, SceneTransitionController, PauseManager, lc);
			PlayerManager = new PlayerManager(Player, pc, xc, UnityContext.Instance, TempObjectsRoot, ProfileController);
			LevelGoalManager = new LevelGoalManager(PlayerManager, LevelManager, lc, xc,
				GameController.LeaderboardController, ProfileController.ActiveInstance);
			CoreWindowsManager.Init(this, PauseManager, LevelManager, LevelGoalManager, PlayerManager, pc, xc, PrefabsController);
			MinimapManager = new MinimapManager(MinimapCamera);
			var lg = new LevelGenerator(this);
			await lg.GenerateLevel();
			PlayerCameraFollower.Init(MainCamera, Player.transform, lg.AreaRect);
			InitComponents();
			// Settings for smooth gameplay
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
			QualitySettings.vSyncCount  = 0;
			PlayerController.OnRespawned += CoreWindowsManager.ShowGetReadyWindow;
			CoreWindowsManager.ShowGetReadyWindow();
		}

		void OnDestroy() {
			PlayerController.OnRespawned -= CoreWindowsManager.ShowGetReadyWindow;
			PauseManager?.Deinit();
			if ( DebugGuiController.HasInstance ) {
				DebugGuiController.Instance.SetDrawable(null);
			}
			LevelManager?.Deinit();
			PlayerManager?.Deinit();
		}
	}
}
