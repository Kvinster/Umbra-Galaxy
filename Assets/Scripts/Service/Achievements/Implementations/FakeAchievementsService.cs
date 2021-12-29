using UnityEngine;

namespace STP.Service.Achievements.Implementations {
	public class FakeAchievementsService : IAchievementsService {
		public void SetAchievement(string achievementName) {
			Debug.Log($"Mock. set achievement {achievementName}");
		}

		public void ResetAllStats() {
			Debug.Log($"Mock. reset all achievements");
		}
	}
}