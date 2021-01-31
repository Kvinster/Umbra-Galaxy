using UnityEngine;

using System.Collections.Generic;

using STP.Core.State;

namespace STP.Core {
	public sealed class ProfileController {
		public static ProfileController ActiveInstance { get; private set; }

		public static bool IsActiveInstanceExists => (ActiveInstance != null);

		readonly List<BaseStateController> _controllers = new List<BaseStateController>();

		readonly ProfileState _profileState;

		public ChunkController   ChunkController   { get; }
		public LevelController   LevelController   { get; }
		public PlayerController  PlayerController  { get; }
		public XpController      XpController      { get; }
		public PowerUpController PowerUpController { get; }

		ProfileController(ProfileState profileState) {
			_profileState = profileState;

			ChunkController   = AddController(new ChunkController(_profileState));
			LevelController   = AddController(new LevelController(_profileState));
			PlayerController  = AddController(new PlayerController(_profileState));
			XpController      = AddController(new XpController(_profileState));
			PowerUpController = AddController(new PowerUpController(_profileState));
		}

		public void Deinit() {
			foreach ( var controller in _controllers ) {
				controller.Deinit();
			}
		}

		T AddController<T>(T controller) where T : BaseStateController {
			_controllers.Add(controller);
			return controller;
		}

		public static ProfileController CreateNewActiveInstance(ProfileState profileState) {
			if ( IsActiveInstanceExists ) {
				Debug.LogError("Another ProfileController active instance exists");
				return ActiveInstance;
			}
			ActiveInstance = new ProfileController(profileState);
			return ActiveInstance;
		}

		public static void ReleaseActiveInstance() {
			if ( !IsActiveInstanceExists ) {
				Debug.LogError("No ProfileController active instance exists");
				return;
			}
			ActiveInstance = null;
		}
	}
}
