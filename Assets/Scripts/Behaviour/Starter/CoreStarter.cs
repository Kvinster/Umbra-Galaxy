using UnityEngine;

using System;
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
		[NotNull]
		public Camera MainCamera;

		[NotNull]
		public RestrictedTransformFollower PlayerCameraFollower;

		[NotNull]
		public Camera MinimapCamera;

		[NotNull]
		public Transform PlayerStartPos;

		[NotNull]
		public CoreWindowsManager CoreWindowsManager;

		[NotNull]
		public SceneTransitionController SceneTransitionController;

		[NotNull]
		public Transform LevelObjectsRoot;

		[NotNull]
		public Transform TempObjectsRoot;

		[NotNull]
		public Transform BordersRoot;

		[NotNull] public GameObject MiniMapObject;
		
		bool _isLevelInitStarted;

		public Player           Player           { get; set; }
		public CoreSpawnHelper  SpawnHelper      { get; private set; }
		public PauseManager     PauseManager     { get; private set; }
		public LevelManager     LevelManager     { get; private set; }
		public PlayerManager    PlayerManager    { get; private set; }
		public LevelGoalManager LevelGoalManager { get; private set; }
		public MinimapManager   MinimapManager   { get; private set; }
		public ShipCreator      ShipCreator      { get; private set; }

		public GameController    GameController    => GameController.Instance;
		public PlayerController  PlayerController  => GameController.PlayerController;
		public XpController      XpController      => GameController.XpController;
		public PrefabsController PrefabsController => GameController.PrefabsController;
		public LevelController   LevelController   => GameController.LevelController;

		void OnDisable() {
			GameController.Deinit();
			LevelGoalManager.Deinit();
		}

		IEnumerator Start() {
			yield return null;
			if ( !_isLevelInitStarted ) {
				// to properly initialize when started from editor
				#pragma warning disable 4014
				UniTaskScheduler.UnobservedTaskException += RaiseUnhandledException;
				InitLevel();
				#pragma warning restore 4014
			}
		}

		void RaiseUnhandledException(Exception e) {
			Debug.LogException(e);
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
				GameController.CreateGameController(GameState.ActiveInstance);
				LevelController.StartLevel(0);
			}
#endif
			ShipCreator = new ShipCreator(LevelObjectsRoot, GameController.PrefabsController);
			var pc = GameController.PlayerController;
			var lc = GameController.LevelController;
			var xc = GameController.XpController;
			pc.OnLevelStart();
			SpawnHelper   = new CoreSpawnHelper(this, TempObjectsRoot);
			PauseManager  = new PauseManager();
			Player        = ShipCreator.CreatePlayerShip(PlayerController.Ship);
			LevelManager  = new LevelManager(Player.transform, SceneTransitionController, PauseManager, lc);
			PlayerManager = new PlayerManager(Player, pc, xc, UnityContext.Instance, TempObjectsRoot);
			LevelGoalManager = new LevelGoalManager(PlayerManager, LevelManager, lc, xc,
				GameController.LeaderboardController);
			CoreWindowsManager.Init(this, PauseManager, LevelManager, LevelGoalManager, PlayerManager, pc, xc,
				PrefabsController);
			MinimapManager = new MinimapManager(MinimapCamera);
			var lg = new LevelGenerator(this);
			await lg.GenerateLevel();
			PlayerCameraFollower.Init(MainCamera, Player.transform, lg.AreaRect);
			InitComponents();
			// Settings for smooth gameplay
			Application.targetFrameRate  =  Screen.currentResolution.refreshRate;
			QualitySettings.vSyncCount   =  0;
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
