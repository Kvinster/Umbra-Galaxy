using System.Collections.Generic;

using STP.Core.State;

namespace STP.Core {
	public sealed class ProfileController {
		readonly List<BaseStateController> _controllers = new List<BaseStateController>();

		readonly ProfileState _profileState;

		public ChunkController       ChunkController       { get; }
		public LevelController       LevelController       { get; }
		public PlayerController      PlayerController      { get; }
		public XpController          XpController          { get; }
		public PowerUpController     PowerUpController     { get; }

		public ProfileController(ProfileState profileState) {
			_profileState = profileState;

			ChunkController       = AddController(new ChunkController(_profileState));
			LevelController       = AddController(new LevelController(_profileState));
			PlayerController      = AddController(new PlayerController(_profileState));
			XpController          = AddController(new XpController(_profileState));
			PowerUpController     = AddController(new PowerUpController(_profileState));
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
	}
}
