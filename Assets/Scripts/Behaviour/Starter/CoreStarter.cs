using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using STP.Behaviour.Core;
using STP.Core;
using STP.Core.State;
using STP.Manager;
using STP.Service;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Starter {
	public sealed class CoreStarter : BaseStarter<CoreStarter> {
		[Header("Parameters")]
		public Rect AreaRect;
		[Header("Dependencies")]
		[NotNull] public Player    Player;
		[NotNull] public Transform PlayerStartPos;
		[NotNull] public Transform LevelObjectsRoot;

		CoreCommonStarter _commonStarter;

		bool _isLevelInitStarted;

		public CoreSpawnHelper  SpawnHelper      { get; private set; }
		public PauseManager     PauseManager     { get; private set; }
		public LevelManager     LevelManager     { get; private set; }
		public PlayerManager    PlayerManager    { get; private set; }
		public LevelGoalManager LevelGoalManager { get; private set; }
		public MinimapManager   MinimapManager   { get; private set; }

		public Camera     MainCamera      => _commonStarter.MainCamera;
		public GameObject MiniMapObject   => _commonStarter.MiniMapObject;
		public Transform  TempObjectsRoot => _commonStarter.TempObjectsRoot;

		public GameController    GameController    => GameController.Instance;
		public PlayerController  PlayerController  => GameController.PlayerController;
		public ScoreController      ScoreController      => GameController.ScoreController;
		public PrefabsController PrefabsController => GameController.PrefabsController;
		public LevelController   LevelController   => GameController.LevelController;

		void OnDisable() {
			GameController.Deinit();
			LevelGoalManager.Deinit();
		}

		protected override void Awake() {
			base.Awake();
#if UNITY_EDITOR
			if ( SceneManager.sceneCount == 1 ) {
				SceneService.CheatLoadLevelCommonScene();
			}
#endif
		}

		void Start() {
			if ( !_isLevelInitStarted ) {
				// to properly initialize when started from editor
				InitLevel();
			}
		}

		void InitLevel() {
			_isLevelInitStarted = true;
			_commonStarter      = CoreCommonStarter.Instance;
			Assert.IsTrue(_commonStarter, "Couldn't find CoreCommonStarter instance");
			SceneManager.MoveGameObjectToScene(_commonStarter.TempObjectsRoot.gameObject, SceneManager.GetActiveScene());
			SceneManager.MoveGameObjectToScene(_commonStarter.BordersRoot.gameObject, SceneManager.GetActiveScene());
			_commonStarter.TempObjectsRoot.position = Vector3.zero;
			_commonStarter.BordersRoot.position     = AreaRect.center;
			_commonStarter.BordersRoot.localScale   = new Vector3(AreaRect.width, AreaRect.height, 1);
#if UNITY_EDITOR
			if ( !GameState.IsActiveInstanceExists ) {
				Debug.Log("Trying to load GameState instance");
				var gs = GameState.TryLoadActiveGameState();
				if ( gs == null ) {
					Debug.Log("Loading failed, creating new GameState instance");
					GameState.CreateNewActiveGameState();
				}
				GameController.CreateGameController(GameState.ActiveInstance);
				LevelController.StartLevel(SceneService.GetLevelIndexFromSceneName());
			}
#endif
			var pc = GameController.PlayerController;
			var lc = GameController.LevelController;
			var xc = GameController.ScoreController;
			pc.OnLevelStart();
			SpawnHelper   = new CoreSpawnHelper(this, _commonStarter.TempObjectsRoot);
			PauseManager  = new PauseManager();
			LevelManager = new LevelManager(Player.transform, _commonStarter.SceneTransitionController, PauseManager,
				lc);
			PlayerManager = new PlayerManager(Player, pc, xc, UnityContext.Instance, _commonStarter.TempObjectsRoot);
			LevelGoalManager =
				new LevelGoalManager(PlayerManager, LevelManager, lc);
			_commonStarter.CoreWindowsManager.Init(PauseManager, LevelManager, LevelGoalManager, PlayerManager, pc, xc,
				GameController.LeaderboardController);
			MinimapManager = new MinimapManager(_commonStarter.MinimapCamera);
			_commonStarter.PlayerCameraFollower.Init(_commonStarter.MainCamera, Player.transform, AreaRect);
			InitComponents();
			PlayerController.OnRespawned += _commonStarter.CoreWindowsManager.ShowGetReadyWindow;
			_commonStarter.CoreWindowsManager.ShowGetReadyWindow();

			// Settings for smooth gameplay
			Application.targetFrameRate  =  Screen.currentResolution.refreshRate;
			QualitySettings.vSyncCount   =  0;
		}

		void OnDestroy() {
			PlayerController.OnRespawned -= _commonStarter.CoreWindowsManager.ShowGetReadyWindow;
			PauseManager?.Deinit();
			LevelManager?.Deinit();
			PlayerManager?.Deinit();
		}

		void OnDrawGizmos() {
			// drawing future game area
			var color = Gizmos.color;
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(AreaRect.center, AreaRect.size);
			Gizmos.color = color;
		}
	}
}
