namespace STP.Service.Achievements.Implementations {
	public interface IAchievementsService {
		void SetAchievement(string achievementName);

		void ResetAllStats();

		int GetStatValue(string statName);

		void SetStatValue(string statName, int value);

		void IncrementStatValue(string statName, int value);
	}
}