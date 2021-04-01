using UnityEngine;

using System.Collections.Generic;

using STP.Core.State;

namespace STP.Core {
	public sealed class ProfileController {
		public static ProfileController ActiveInstance { get; private set; }

		public static bool IsActiveInstanceExists => (ActiveInstance != null);

		readonly List<BaseStateController> _controllers = new List<BaseStateController>();

		public string ProfileName => ProfileState.ProfileName;

		public ProfileState ProfileState { get; }

		public LevelController   LevelController   { get; }
		public PlayerController  PlayerController  { get; }
		public XpController      XpController      { get; }
		public PowerUpController PowerUpController { get; }

		ProfileController(ProfileState profileState) {
			ProfileState = profileState;

			LevelController   = AddController(new LevelController(ProfileState));
			PlayerController  = AddController(new PlayerController(ProfileState));
			XpController      = AddController(new XpController());
			PowerUpController = AddController(new PowerUpController(ProfileState));
		}

		public void Deinit() {
			foreach ( var controller in _controllers ) {
				controller.Deinit();
			}
		}

		public void Save() {
			ProfileState.Save();
		}

		public void StartLevel(int levelIndex) {
			LevelController.StartLevel(levelIndex);
		}

		T AddController<T>(T controller) where T : BaseStateController {
			_controllers.Add(controller);
			return controller;
		}

		public static ProfileController CreateNewActiveInstance(ProfileState profileState) {
			if ( IsActiveInstanceExists ) {
				Debug.LogError("Another ProfileController active instance exists");
				ReleaseActiveInstance();
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
