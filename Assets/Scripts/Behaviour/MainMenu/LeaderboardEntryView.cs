using STP.Core.Leaderboards;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.MainMenu {
	public sealed class LeaderboardEntryView : GameComponent {
		const string DescFormat = "{0}: {1} XP";

		[NotNull] public TMP_Text DescText;

		public void Init(Score leaderboardEntry) {
			DescText.text = string.Format(DescFormat, leaderboardEntry.UserName, leaderboardEntry.ScoreValue);
		}

		public void Deinit() { }
	}
}
