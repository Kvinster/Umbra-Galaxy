using UnityEngine.Assertions;

using STP.Core.State;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.MainMenu {
	public sealed class LeaderboardEntryView : GameComponent {
		const string DescFormat = "{0}: {1} XP";

		[NotNull] public TMP_Text DescText;

		public void Init(LeaderboardEntry leaderboardEntry) {
			Assert.IsNotNull(leaderboardEntry);

			DescText.text = string.Format(DescFormat, leaderboardEntry.ProfileName, leaderboardEntry.Highscore);
		}

		public void Deinit() { }
	}
}
