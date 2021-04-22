using UnityEngine;

using System;

using NaughtyAttributes;

namespace STP.Config {
	[Serializable]
	public sealed class AsteroidSpawnerSettings : BaseSpawnerSettings {
		[EnableIf(nameof(Enabled))] [AllowNesting] [Space]
		public float AsteroidSpeed;
	}
}
