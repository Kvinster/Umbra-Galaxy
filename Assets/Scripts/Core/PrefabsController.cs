using UnityEngine;
using UnityEngine.Assertions;

using STP.Behaviour.Core;
using STP.Common;
using STP.Config;
using STP.Core.State;

namespace STP.Core {
	public sealed class PrefabsController : BaseStateController {
		readonly PrefabConfig _config;

		public PrefabsController(ProfileState profileState) {
			_config = LoadConfig();
			Assert.IsNotNull(_config);
		}

		public GameObject GetPowerUpPrefab(PowerUpType type) {
			return _config.GetPowerUpPrefab(type);
		}

		public GameObject GetShipPrefab(ShipType type) {
			return _config.GetShipPrefab(type);
		}
		public Sprite GetShipPreview(ShipType type) {
			return _config.GetShipPreviewSprite(type);
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