using UnityEngine;

using System;
using System.Collections.Generic;
using System.IO;

using STP.Common;

namespace STP.Config {
	[Serializable]
	public class LevelInfo {
		public const int GeneratorCellSize       = 100;
		public const int IdleEnemyGroupChunkSize = 1500;
		
		public int               GeneratorsCount    = 1;
		public int               GeneratorsSideSize = 7;
		public int               EnemyGroupsCount   = 0;
		public List<PowerUpType> PowerUps;
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
		
		
		#if UNITY_EDITOR
		[ContextMenu("Populate from csv")]
		public void PopulateFromConfig() {
			var config = Resources.Load<TextAsset>("Balance");
			Levels.Clear();
			using ( var reader = new StringReader(config.text) ) {
				for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
					var elements    = line.Split(',');
					var powerUps = new List<PowerUpType>();
					// Parsing powerups
					var powerUpNames = elements[4].Split(' ');
					foreach ( var powerUpName in powerUpNames ) {
						if ( string.IsNullOrEmpty(powerUpName) ) {
							continue;
						}
						if ( Enum.TryParse(powerUpName, out PowerUpType powerUp) ) {
							powerUps.Add(powerUp);
						}
						else {
							Debug.LogError($"Can't parse powerup name {powerUpName}");
						} 
					}	
					// Creating level info
					var levelInfo = new LevelInfo {
						GeneratorsCount    = int.Parse(elements[0]),
						GeneratorsSideSize = int.Parse(elements[1]),
						EnemyGroupsCount   = int.Parse(elements[2]),
						PowerUps           = powerUps
					};
					Levels.Add(levelInfo);
				} 
				
			}
			
		}
		
		#endif
	}
	
}