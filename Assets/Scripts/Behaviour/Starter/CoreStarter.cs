using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using STP.Behaviour.Core;
using STP.Behaviour.Core.Enemy;
using STP.Behaviour.Utils;
using STP.Config;
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


		[Header("optional dependencies")]
		public BaseBoss Boss;

		CoreCommonStarter _commonStarter;

		bool _isLevelInitStarted;

		public CoreSpawnHelper  SpawnHelper      { get; private set; }
		public PauseManager     PauseManager     { get; private set; }
		public LevelManager     LevelManager     { get; private set; }
		public PlayerManager    PlayerManager    { get; private set; }
		public LevelGoalManager LevelGoalManager { get; private set; }
		public MinimapManager   MinimapManager   { get; private set; }

		public Camera             MainCamera      => _commonStarter.MainCamera;
		public GameObject         MiniMapObject   => _commonStarter.MiniMapObject;
		public Transform          TempObjectsRoot => _commonStarter.TempObjectsRoot;
		public Transform          BordersRoot     => _commonStarter.BordersRoot;
		public Portal             Portal          => _commonStarter.Portal;
		public CoreWindowsManager WindowsManager  => _commonStarter.CoreWindowsManager;
		public CameraShake        CameraShake     => _commonStarter.CameraShake;

		public GameController    GameController    => GameController.Instance;
		public PlayerController  PlayerController  => GameController.PlayerController;
		public ScoreController   ScoreController   => GameController.ScoreController;
		public PrefabsController PrefabsController => GameController.PrefabsController;
		public LevelController   LevelController   => GameController.LevelController;

		public Rect CameraArea {
			get {
				var center = (Vector2)MainCamera.transform.position;
				var size   = new Vector2(MainCamera.aspect * MainCamera.orthographicSize * 2, MainCamera.orthographicSize * 2);
				return new Rect(center - size / 2, size);
			}
		}

		void OnDisable() {
			GameController.Deinit();
			LevelGoalManager.Deinit();
		}

		protected override void Awake() {
			base.Awake();
#if UNITY_EDITOR
			if ( !SceneService.IsLevelCommonSceneLoaded  ) {
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
			SceneManager.MoveGameObjectToScene(TempObjectsRoot.gameObject, SceneManager.GetActiveScene());
			SceneManager.MoveGameObjectToScene(BordersRoot.gameObject, SceneManager.GetActiveScene());

			TempObjectsRoot.position = Vector3.zero;
			BordersRoot.position     = AreaRect.center;
			BordersRoot.localScale   = new Vector3(AreaRect.width, AreaRect.height, 1);


#if UNITY_EDITOR
			if ( !GameState.IsActiveInstanceExists ) {
				Debug.Log("Trying to load GameState instance");
				var gs = GameState.TryLoadActiveGameState();
				if ( gs == null ) {
					Debug.Log("Loading failed, creating new GameState instance");
					GameState.CreateNewActiveGameState();
				}
				GameController.CreateGameController(GameState.ActiveInstance);
				LevelController.StartLevelFromEditor();
			}
#endif

			BordersRoot.gameObject.SetActive(LevelController.CurLevelType == LevelType.Regular);
			var pc = GameController.PlayerController;
			var lc = GameController.LevelController;
			var xc = GameController.ScoreController;
			pc.OnLevelStart();
			SpawnHelper   = new CoreSpawnHelper(this, TempObjectsRoot);
			PauseManager  = new PauseManager();
			LevelManager = new LevelManager(Player, _commonStarter.SceneTransitionController, PauseManager, lc,
				WindowsManager);
			PlayerManager = new PlayerManager(Player, pc, xc, UnityContext.Instance, TempObjectsRoot);
			LevelGoalManager =
				new LevelGoalManager(PlayerManager, LevelManager, lc);
			MinimapManager = new MinimapManager(_commonStarter.MinimapCamera);
			if ( LevelController.CurLevelType == LevelType.Regular ) {
				_commonStarter.PlayerCameraFollower.Init(_commonStarter.MainCamera, Player.transform, AreaRect);
			}
			TryRecalcArea();
			InitComponents();
			WindowsManager.Init(PauseManager, LevelManager, PlayerManager, MinimapManager, lc, pc, xc, GameController.LeaderboardController, Player, Boss);
			_commonStarter.Portal.Init(this);

			// Settings for smooth gameplay
			Application.targetFrameRate  =  Screen.currentResolution.refreshRate;
			QualitySettings.vSyncCount   =  0;

			if ( LevelController.CurLevelType == LevelType.Boss ) {
				InitAsBossLevel();
			}
		}

		void InitAsBossLevel() {
			if ( MainCamera.TryGetComponent<RestrictedTransformFollower>(out var comp) ) {
				comp.enabled = false;
			}
			MiniMapObject.SetActive(false);
		}

		void TryRecalcArea() {
			if (LevelController.CurLevelType != LevelType.Boss) {
				return;
			}
			var cam        = MainCamera;
			var areaHeight = cam.orthographicSize * 2;
			var areaWidth  = cam.aspect * areaHeight;
			AreaRect = new Rect(new Vector2(-areaWidth / 2, -areaHeight / 2), new Vector2(areaWidth, areaHeight));
		}

		void OnDestroy() {
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