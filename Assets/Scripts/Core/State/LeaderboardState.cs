using System.Collections.Generic;
using System.Xml;

using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class LeaderboardState : BaseState {
		List<LeaderboardEntry> _entries = new List<LeaderboardEntry>();

		public List<LeaderboardEntry> Entries => new List<LeaderboardEntry>(_entries);

		public override string Name => "leaderboard";

		public override void Load(XmlNode node) {
			_entries = node.LoadNodeList("entries", "entry", () => new LeaderboardEntry()) ?? new List<LeaderboardEntry>();
			_entries.Sort((a, b) => b.Highscore.CompareTo(a.Highscore));
		}

		public override void Save(XmlElement elem) {
			elem.SaveNodeList("entries", "entry", _entries);
		}

		public void AddEntry(string profileName, int highscore) {
			_entries.Add(new LeaderboardEntry {
				ProfileName = profileName,
				Highscore   = highscore
			});
			_entries.Sort((a, b) => b.Highscore.CompareTo(a.Highscore));
		}

		public void Clear() {
			_entries.Clear();
		}
	}
}
