using UnityEngine;
using UnityEngine.Assertions;

using STP.Behaviour.Core;
using STP.Behaviour.Utils;
using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;
using STP.View.DebugGUI;

namespace STP.Behaviour.Starter {
	public sealed class CoreCommonStarter : GameComponent {
		static CoreCommonStarter _instance;

		public static CoreCommonStarter Instance {
			get {
				if ( !_instance ) { // for when launching the game from editor from the level scene
					_instance = FindObjectOfType<CoreCommonStarter>();
				}
				return _instance;
			}
			private set => _instance = value;
		}

		void OnEnable() {
			Assert.IsTrue(!Instance || (Instance == this), "Another instance of CoreCommonStarter already exists");
			Instance = this;
		}

		void OnDisable() {
			Assert.IsTrue(Instance == this, "Instance == this");
			Instance = null;
		}

		[NotNull] public Camera                      MainCamera;
		[NotNull] public RestrictedTransformFollower PlayerCameraFollower;
		[NotNull] public Camera                      MinimapCamera;
		[NotNull] public CoreWindowsManager          CoreWindowsManager;
		[NotNull] public SceneTransitionController   SceneTransitionController;
		[NotNull] public Transform                   TempObjectsRoot;
		[NotNull] public Transform                   BordersRoot;
		[NotNull] public GameObject                  MiniMapObject;

		bool _isLevelInitStarted;

		void OnDestroy() {
			if ( DebugGuiController.HasInstance ) {
				DebugGuiController.Instance.SetDrawable(null);
			}
		}
	}
}
