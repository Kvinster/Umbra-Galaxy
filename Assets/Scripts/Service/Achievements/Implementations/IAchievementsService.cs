namespace STP.Service.Achievements.Implementations {
	public interface IAchievementsService {
		void SetAchievement(string achievementName);

		void ResetAllStats();
	}
}