using UnityEngine;

namespace STP.Service.Achievements.Implementations {
	public class FakeAchievementsService : IAchievementsService {
		public void SetAchievement(string achievementName) {
			Debug.Log($"Mock. set achievement {achievementName}");
		}

		public void ResetAllStats() {
			Debug.Log("Mock. reset all achievements");
		}

		public int GetStatValue(string statName) {
			Debug.Log($"Mock. returning 0 for stat {statName}");
			return 0;
		}

		public void SetStatValue(string statName, int value) {
			Debug.Log($"Mock. Setting {value} for stat {statName}");
		}

		public void IncrementStatValue(string statName, int value) {
			var statValue = GetStatValue(statName);
			SetStatValue(statName, statValue + value);
		}
	}
}