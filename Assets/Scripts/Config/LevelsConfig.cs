using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Common;
using STP.Utils.Attributes;

namespace STP.Config {
	[Serializable]
	public class LevelInfo {
		public int LevelSeed = 0;

		public int                   GeneratorsCount = 1;
		public int                   CellSize = 1500;
		public List<PowerUpInfo>     PowerUpInfos;
		public List<ChunkWeightInfo> Chunks;

		public int LevelSpaceSize;
	}

	public class WeightedValue {
		public int Weight;
	}

	[Serializable]
	public class PowerUpInfo {
		public PowerUpType PowerUpType;
		public int         Count;
	}

	[Serializable]
	public class ChunkWeightInfo : WeightedValue {
		[ChunkName]
		public string Name;
	}

	[CreateAssetMenu(fileName = "AllLevels", menuName = "ScriptableObjects/LevelsConfig", order = 1)]
	public class LevelsConfig : ScriptableObject {
		public List<LevelInfo> Levels;

		public LevelInfo GetLevelConfig(int levelIndex) {
			if ( (levelIndex < 0) || (levelIndex >= Levels.Count) ) {
				Debug.LogErrorFormat("Invalid level index '{0}'", levelIndex);
				return null;
			}
			return Levels[levelIndex];
		}
	}
}