using System;
using NaughtyAttributes;
using UnityEngine;

namespace STP.Config.SpawnerSettings {
	[Serializable]
	public sealed class AsteroidSpawnerSettings : BaseSpawnerSettings {
		[EnableIf(nameof(Enabled))] [AllowNesting] [Space]
		public float AsteroidMinSpeed;

		[EnableIf(nameof(Enabled))] [AllowNesting]
		public float AsteroidMaxSpeed;
	}
}
