using System.Collections.Generic;

using STP.Common;

namespace STP.Config {
	public sealed class RegularLevelInfo : BaseLevelInfo {
		public const int GeneratorCellSize       = 100;
		public const int IdleEnemyGroupChunkSize = 1500;

		public int               GeneratorsCount    = 1;
		public int               GeneratorsSideSize = 7;
		public int               EnemyGroupsCount   = 0;
		public List<PowerUpType> PowerUps;

		public override LevelType LevelType => LevelType.Regular;
	}
}
