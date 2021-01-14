using UnityEngine;

using System;
using System.Collections.Generic;

namespace STP.Config {
	[Serializable]
	public class LevelInfo {
		public int GeneratorsCount = 1;
		public int LevelSeed       = 0;

		public int LevelSpaceSize;
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