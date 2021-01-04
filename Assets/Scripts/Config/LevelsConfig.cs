using UnityEngine;
using System;
using System.Collections.Generic;

namespace STP.Config {
	[Serializable]
	public class LevelInfo {
		public int    GeneratorsCount = 1;
		public int    LevelSeed       = 0;
		public string LevelName       = "Level1";
		public string NextLevelName   = "Level2";
	}
	
	[CreateAssetMenu(fileName = "AllLevels", menuName = "ScriptableObjects/LevelsConfig", order = 1)]
	public class LevelsConfig : ScriptableObject {
		public List<LevelInfo> Levels;

		public LevelInfo GetLevelConfig(string levelName) {
			return Levels.Find(x => (x.LevelName == levelName));
		}
	}
}