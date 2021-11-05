namespace STP.Core.Leaderboards {
	public readonly struct Score {
		public readonly int    Rank;
		public readonly long   ScoreValue;
		public readonly string UserName;

		public Score(int rank, long score, string userName) {
			Rank       = rank;
			ScoreValue = score;
			UserName   = userName;
		}
	}
}