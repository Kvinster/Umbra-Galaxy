#if !DISABLESTEAMWORKS

using Steamworks;

namespace STP.Service.Achievements.Implementations {

	public class SteamworksAchievementsService : IAchievementsService {
		public void SetAchievement(string achievementName) {
			if (!SteamManager.Initialized) {
				return;
			}

			SteamUserStats.SetAchievement(achievementName);
			SteamUserStats.StoreStats();
		}

		public void ResetAllStats() {
			if (!SteamManager.Initialized) {
				return;
			}
			
			SteamUserStats.ResetAllStats(true);
		}
	}
}
#endif