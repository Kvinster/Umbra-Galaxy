using UnityEngine;

using System.Collections.Generic;
using System.Xml;

using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class LeaderboardState : IXmlNodeSerializable {
		List<LeaderboardEntry> _entries = new List<LeaderboardEntry>();

		public List<LeaderboardEntry> Entries => new List<LeaderboardEntry>(_entries);

		public void AddEntry(string profileName, int highscore) {
			_entries.Add(new LeaderboardEntry {
				ProfileName = profileName,
				Highscore   = highscore
			});
			Save();
		}

		public void Clear() {
			_entries.Clear();
			Save();
		}

		public void Load(XmlNode node) {
			_entries = node.LoadNodeList("entries", "entry", () => new LeaderboardEntry()) ??
			           new List<LeaderboardEntry>();
			_entries.Sort((a, b) => b.Highscore.CompareTo(a.Highscore));
		}

		public void Save() {
			var document = new XmlDocument();
			var root     = document.CreateElement("root");
			document.AppendChild(root);
			Save(root);
			document.SaveLeaderboardDocument();
		}

		public void Save(XmlElement elem) {
			elem.SaveNodeList("entries", "entry", _entries);
		}

		public static LeaderboardState GetInstance() {
			return TryLoadInstance() ?? CreateInstance();
		}

		static LeaderboardState TryLoadInstance() {
			var document = XmlUtils.LoadLeaderboardDocument();
			if ( document == null ) {
				return null;
			}
			var root = document.SelectSingleNode("root");
			if ( root == null ) {
				Debug.LogError("Root is null");
				return null;
			}
			var instance = new LeaderboardState();
			instance.Load(root);
			return instance;
		}

		static LeaderboardState CreateInstance() {
			var instance = new LeaderboardState();
			instance.Save();
			return instance;
		}
	}
}
