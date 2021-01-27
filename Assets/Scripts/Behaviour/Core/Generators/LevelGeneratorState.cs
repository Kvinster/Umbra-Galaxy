using System.Collections.Generic;

using STP.Common;

namespace STP.Behaviour.Core.Generators {
	public class LevelGeneratorState {
		public Dictionary<PowerUpType, int> CreatedPowerUpsCount = new Dictionary<PowerUpType, int>();

		public int GetOrDefaultCreatedPowerUpsCount(PowerUpType powerUpType, int defaultValue = 0) {
			return CreatedPowerUpsCount.ContainsKey(powerUpType) ? CreatedPowerUpsCount[powerUpType] : defaultValue;
		}
	}
}