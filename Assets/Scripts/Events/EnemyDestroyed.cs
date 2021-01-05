namespace STP.Events {
	public struct EnemyDestroyed {
		public string EnemyName;

		public EnemyDestroyed(string enemyName) {
			EnemyName = enemyName;
		}
	}
}