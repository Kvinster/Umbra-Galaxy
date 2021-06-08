using System;

namespace STP.Core.ShootingsSystems.Generator {
	[Serializable]
	public sealed class GeneratorShootingSystemParams : ShootingSystemParams {
		public float MinBulletSpeed   = 1f;
		public float MaxSpeedDegradationDistance = 1500f;
	}
}