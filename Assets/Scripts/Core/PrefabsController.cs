using UnityEngine;
using UnityEngine.Assertions;

using STP.Common;
using STP.Config;

namespace STP.Core {
	public sealed class PrefabsController : BaseStateController {
		readonly PrefabConfig _config;

		public PrefabsController() {
			_config = LoadConfig();
			Assert.IsNotNull(_config);
		}

		public GameObject GetPowerUpPrefab(PowerUpType type) {
			return _config.GetPowerUpPrefab(type);
		}

		public GameObject GetBulletPrefab(bool isEnhanced) {
			var bulletType = isEnhanced ? BulletType.Enhanced : BulletType.Default;
			return _config.GetBulletPrefab(bulletType);
		}

		static PrefabConfig LoadConfig() {
			return Resources.Load<PrefabConfig>("PrefabsConfig");
		}
	}
}