namespace STP.Core.Leaderboards {
	public readonly struct Score {
		public readonly int    Rank;
		public readonly long   ScoreValue;
		public readonly string UserName;
		public readonly string Id;

		public Score(int rank, long score, string userName, string id) {
			Rank       = rank;
			ScoreValue = score;
			UserName   = userName;
			Id         = id;
		}
	}
}