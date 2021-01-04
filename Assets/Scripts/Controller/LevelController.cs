using UnityEngine;

using STP.Config;
using STP.Utils;

namespace STP.Controller {
	public class LevelController : Singleton<LevelController> {
		const string DefaultLevelName = "Level1";

		LevelsConfig _levelsConfig;

		public string CurLevelName { get; private set; }
		
		public LevelController() {
			CurLevelName = DefaultLevelName;
			LoadConfig();
		}

		public void ChangeLevel(string newLevelName) {
			CurLevelName = newLevelName;
		}

		public LevelInfo GetCurLevelConfig() {
			return _levelsConfig.GetLevelConfig(CurLevelName);
		}

		void LoadConfig() {
			_levelsConfig = Resources.Load<LevelsConfig>("AllLevels");
		}
	}
}