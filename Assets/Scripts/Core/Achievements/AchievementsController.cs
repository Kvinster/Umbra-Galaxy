using System.Collections.Generic;
using STP.Events;
using STP.Manager;
using STP.Service.Achievements;
using STP.Service.Achievements.Implementations;
using STP.Utils.Events;

namespace STP.Core.Achievements {
	public sealed class AchievementsController : BaseStateController {
		readonly IAchievementsService _achievementsService;

		readonly Dictionary<string, string> _enemyToAchievementTypeConverter = new Dictionary<string, string> {
			{"RailgunBoss", AchievementType.KilledFirstBoss},
			{"SpawnerBoss", AchievementType.KilledSecondBoss},
			{"Asteroid", AchievementType.DestroyAsteroid},
		};
		
		readonly Dictionary<string, string> _enemyToStatTypeConverter = new Dictionary<string, string> {
			{"MainGenerator", StatType.DestroyedGenerators},
		};

		public AchievementsController() {
			_achievementsService = AchievementServiceProvider.Implementation;
			_achievementsService.SetAchievement(AchievementType.StartGame);
			EventManager.Subscribe<EnemyDestroyed>(OnEnemyDestroyed);	
		}
		
		public override void Deinit() {
			base.Deinit();
			EventManager.Unsubscribe<EnemyDestroyed>(OnEnemyDestroyed);
		}
		
		public void OnPlayerDied() {
			_achievementsService.SetAchievement(AchievementType.PlayerDied);
		}
		
		public void OnLeaderboardPlaceDetected(int leaderboardPlace) {
			if (leaderboardPlace == 1) {
				_achievementsService.SetAchievement(AchievementType.TopPlayer);
			}
			if (leaderboardPlace <= 5) {
				_achievementsService.SetAchievement(AchievementType.Top5Player);
			}
			if (leaderboardPlace <= 25) {
				_achievementsService.SetAchievement(AchievementType.Top25Player);
			}
		}

		public void OnGameCompleted() {
			_achievementsService.SetAchievement(AchievementType.CompleteGame);
		}

		void OnEnemyDestroyed(EnemyDestroyed ev) {
			if (_enemyToAchievementTypeConverter.TryGetValue(ev.EnemyName, out var achievementType)) {
				_achievementsService.SetAchievement(achievementType);
				return;
			}
			if (_enemyToStatTypeConverter.TryGetValue(ev.EnemyName, out var statType)){
				_achievementsService.IncrementStatValue(statType, 1);
				return;
			} 
			_achievementsService.IncrementStatValue(StatType.KilledEnemies, 1);
		}
	}
}