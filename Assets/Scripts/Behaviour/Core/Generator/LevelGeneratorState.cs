using System.Collections.Generic;
using STP.Common;
using STP.Config;

namespace STP.Behaviour.Core.Generator {
	public class LevelGeneratorState {
		public Dictionary<PowerUpType, int> CreatedPowerUpsCount = new Dictionary<PowerUpType, int>();
	}
}