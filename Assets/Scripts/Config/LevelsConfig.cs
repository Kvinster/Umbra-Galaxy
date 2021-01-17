using UnityEngine;

using System;
using System.Collections.Generic;
using STP.Common;

namespace STP.Config {
	[Serializable]
	public class LevelInfo {
		public int LevelSeed = 0;

		public int                   GeneratorsCount = 1;
		public List<PowerUpInfo>     PowerUpInfo;
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