using System;
using System.Collections.Generic;

namespace STP.Common {
	public static class PowerUpTypeHelper {
		public static readonly IReadOnlyCollection<PowerUpType> PowerUpTypes;

		static PowerUpTypeHelper() {
			PowerUpTypes = new List<PowerUpType>((PowerUpType[])Enum.GetValues(typeof(PowerUpType)));
		}
	}
}
