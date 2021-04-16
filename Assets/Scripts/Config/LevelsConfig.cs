﻿using UnityEngine;

using System;
using System.Collections.Generic;
using System.IO;

using STP.Common;

namespace STP.Config {
	[CreateAssetMenu(fileName = "AllLevels", menuName = "ScriptableObjects/LevelsConfig", order = 1)]
	public sealed class LevelsConfig : ScriptableObject {
		public List<BaseLevelInfo> Levels;

		public BaseLevelInfo GetLevelConfig(int levelIndex) {
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
					var levelInfo = CreateInstance<RegularLevelInfo>();
					levelInfo.GeneratorsCount    = int.Parse(elements[0]);
					levelInfo.GeneratorsSideSize = int.Parse(elements[1]);
					levelInfo.EnemyGroupsCount   = int.Parse(elements[2]);
					levelInfo.PowerUps           = powerUps;
					UnityEditor.AssetDatabase.CreateAsset(levelInfo, "Assets/Resources/NewLevel.asset");
					Levels.Add(levelInfo);
				}
			}
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
			UnityEditor.EditorUtility.SetDirty(this);

		}
#endif
	}

}