

using STP.Service.Achievements.Implementations;

namespace STP.Service.Achievements {
	public class AchievementServiceProvider  {
		public static IAchievementsService Implementation =>
#if !DISABLESTEAMWORKS
			new SteamworksAchievementsService();
#else
			new FakeAchievementsService();
#endif
	}
}