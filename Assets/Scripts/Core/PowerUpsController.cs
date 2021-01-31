using UnityEngine;
using UnityEngine.Assertions;

using STP.Common;
using STP.Config;
using STP.Core.State;

namespace STP.Core {
	public sealed class PowerUpController : BaseStateController {
		readonly PowerUpConfig _powerUpConfig;

		public PowerUpController(ProfileState profileState) {
			_powerUpConfig = LoadConfig();
			Assert.IsNotNull(_powerUpConfig);
		}

		public GameObject GetPowerUpPrefab(PowerUpType type) {
			return _powerUpConfig.GetPowerUpPrefab(type);
		}


		public static GameObject GetPowerUpPrefabInEditor(PowerUpType type) {
			var config = LoadConfig();
			if ( config ) {
				return config.GetPowerUpPrefab(type);
			}
			Debug.LogError("Can't load powerup config in editor");
			return null;
		}

		static PowerUpConfig LoadConfig() {
			return Resources.Load<PowerUpConfig>("PowerUpConfig");
		}
	}
}