using System.Collections.Generic;

using STP.Common;

namespace STP.Behaviour.Core.Generators.Regular {
	public sealed class RegularLevelGeneratorState {
		public readonly Dictionary<PowerUpType, int> CreatedPowerUpsCount = new Dictionary<PowerUpType, int>();

		public int LevelSideBlocksCount;
		public int LevelChunkSideSize;

		public int GetOrDefaultCreatedPowerUpsCount(PowerUpType powerUpType, int defaultValue = 0) {
			return CreatedPowerUpsCount.ContainsKey(powerUpType) ? CreatedPowerUpsCount[powerUpType] : defaultValue;
		}
	}
}
