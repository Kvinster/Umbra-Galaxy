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

		public int GetStatValue(string statName) {
			return SteamUserStats.GetStat(statName, out int res) ? res : 0;
		}

		public void SetStatValue(string statName, int value) {
			SteamUserStats.SetStat(statName, value);
		}

		public void IncrementStatValue(string statName, int value) {
			var statValue = GetStatValue(statName);
			SetStatValue(statName, statValue + value);
			SteamUserStats.StoreStats();
		}
	}
}
#endif