using System;

namespace STP.Core.ShootingsSystems.Glider {
	[Serializable]
	public sealed class GliderShootingSystemParams : ShootingSystemParams {
		public float MinAttackDistance;
		public float MaxAttackDistance;
	}
}