using UnityEngine;

using System.IO;
using System.Xml;

namespace STP.Utils.Xml {
	public static class XmlUtils {
		public static readonly string BasePath = Path.Combine(Application.persistentDataPath, "saves");

		const string GameStateExtension      = ".stpsave";
		const string LeaderboardRelativePath = "leaderboard.stp";

		public static void SaveLeaderboardDocument(this XmlDocument document) {
			if ( document == null ) {
				Debug.LogError("Document is null");
				return;
			}
			document.SaveDocument(LeaderboardRelativePath);
		}

		public static void SaveGameStateDocument(this XmlDocument document, string profileName) {
			if ( document == null ) {
				Debug.LogError("Document is null");
				return;
			}
			document.SaveDocument(profileName + GameStateExtension);
		}

		public static XmlDocument LoadGameStateDocument(string profileName) {
			if ( string.IsNullOrEmpty(profileName) ) {
				Debug.LogError("Profile name is null or empty");
			}
			return LoadSavedDocument(profileName + GameStateExtension);
		}

		public static XmlDocument LoadLeaderboardDocument() {
			return LoadSavedDocument(LeaderboardRelativePath);
		}

		static void SaveDocument(this XmlDocument document, string relativePath) {
			if ( document == null ) {
				Debug.LogError("Document is null");
				return;
			}
			var di = new DirectoryInfo(BasePath);
			if ( !di.Exists ) {
				di.Create();
			}
			var savePath = Path.Combine(di.ToString(), relativePath);
			Debug.LogFormat("Saving to '{0}'", savePath);
			var fi = new FileInfo(savePath);
			if ( fi.Exists ) {
				fi.Delete();
			}
			document.Save(savePath);
		}

		static XmlDocument LoadSavedDocument(string relativePath) {
			if ( string.IsNullOrEmpty(relativePath) ) {
				Debug.LogError("Relative path is null or empty");
				return null;
			}
			var di = new DirectoryInfo(BasePath);
			if ( !di.Exists ) {
				return null;
			}
			var loadPath = Path.Combine(di.ToString(), relativePath);
			Debug.LogFormat("Loading from: '{0}'", loadPath);
			var fi = new FileInfo(loadPath);
			if ( !fi.Exists ) {
				return null;
			}
			var document = new XmlDocument();
			document.Load(loadPath);
			return document;
		}
	}
}
