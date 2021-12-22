using UnityEngine;
using UnityEngine.Assertions;

using System;
using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using STP.Common;

namespace STP.Config {
	[CreateAssetMenu(fileName = "AllLevels", menuName = "ScriptableObjects/LevelsConfig", order = 1)]
	public sealed class LevelsConfig : ScriptableObject {
		const string Path = "AllLevels";

		static LevelsConfig _instance;

		public static LevelsConfig Instance {
			get {
				if ( !_instance ) {
					_instance = Resources.Load<LevelsConfig>(Path);
					Assert.IsTrue(_instance);
				}
				return _instance;
			}
		}

		[Expandable]
		public List<BaseLevelInfo> LevelScenes;

		public int TotalLevelsCount => LevelScenes.Count;

		public BaseLevelInfo GetLevelConfig(int levelIndex) {
			if ( (levelIndex < 0) || (levelIndex >= LevelScenes.Count) ) {
				Debug.LogErrorFormat("Invalid level index '{0}'", levelIndex);
				return null;
			}
			return LevelScenes[levelIndex];
		}

		public int FindLevelIndexByName(string sceneName) {
			for (var i = 0; i < LevelScenes.Count; i++ ) {
				if (sceneName == LevelScenes[i].SceneName) {
					return i;
				}
			}
			return -1;
		}
	}

}