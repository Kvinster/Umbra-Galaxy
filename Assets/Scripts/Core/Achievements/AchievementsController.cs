using STP.Events;
using STP.Service.Achievements;
using STP.Service.Achievements.Implementations;
using STP.Utils.Events;

namespace STP.Core.Achievements {
	public sealed class AchievementsController : BaseStateController {
		readonly IAchievementsService _achievementsService;

		readonly ScoreController _scoreController;
		
		public AchievementsController(ScoreController scoreController) {
			_scoreController = scoreController;
			_achievementsService = AchievementServiceProvider.Implementation;
			#if UNITY_EDITOR
			// reset stats only for debugging
			AchievementServiceProvider.Implementation.ResetAllStats();
			#endif
			_achievementsService.SetAchievement(AchievementType.StartGame);
			EventManager.Subscribe<EnemyDestroyed>(OnEnemyDestroyed);	
		}

		public override void Deinit() {
			base.Deinit();
			EventManager.Unsubscribe<EnemyDestroyed>(OnEnemyDestroyed);
		}

		void OnEnemyDestroyed(EnemyDestroyed ev) {
			_achievementsService.IncrementStatValue(StatType.KilledEnemies, 1);
			if (_scoreController.IsBossEnemy(ev.EnemyName)) {
				_achievementsService.IncrementStatValue(StatType.KilledBosses, 1);
			}
			switch (ev.EnemyName) {
				case "Generator":
					_achievementsService.IncrementStatValue(StatType.DestroyedGenerators, 1);
					break;
				case "Asteroid":
					_achievementsService.SetAchievement(AchievementType.DestroyAsteroid);
					break;
			}
		}
	}
}