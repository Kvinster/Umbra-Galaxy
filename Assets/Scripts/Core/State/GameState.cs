using UnityEngine;

using System.Collections.Generic;
using System.IO;
using System.Xml;

using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class GameState {
		public static GameState ActiveInstance { get; private set; }

		public static bool IsActiveInstanceExists => (ActiveInstance != null);

		public readonly string ProfileName;

		readonly List<BaseState> _states = new List<BaseState>();

		public PlayerState PlayerState { get; }
		public XpState     XpState     { get; }
		public LevelState  LevelState  { get; }

		GameState(string profileName) {
			ProfileName = profileName;

			PlayerState = AddState(new PlayerState());
			XpState     = AddState(new XpState());
			LevelState  = AddState(new LevelState());
		}

		public void Save() {
			var document = new XmlDocument();
			var root     = document.CreateElement("root");
			document.AppendChild(root);
			foreach ( var state in _states ) {
				var childElement = document.CreateElement(state.Name);
				state.Save(childElement);
				root.AppendChild(childElement);
			}
			document.SaveGameStateDocument(ProfileName);
		}

		void Load(XmlDocument xmlDocument) {
			var root = xmlDocument.DocumentElement;
			foreach ( var state in _states ) {
				var childNode = root.FindChild(state.Name);
				if ( childNode != null ) {
					state.Load(childNode);
				}
			}
		}

		T AddState<T>(T state) where T : BaseState {
			_states.Add(state);
			return state;
		}

		public static GameState CreateNewActiveGameState(string profileName) {
			if ( IsActiveInstanceExists ) {
				Debug.LogError("GameState active instance already exists");
				return ActiveInstance;
			}
			ActiveInstance = new GameState(profileName);
			return ActiveInstance;
		}

		public static GameState LoadGameState(string profileName) {
			var document = XmlUtils.LoadGameStateDocument(profileName);
			if ( document == null ) {
				Debug.LogErrorFormat("Can't load save for profile name '{0}'", profileName);
				return null;
			}
			var gs = new GameState(profileName);
			gs.Load(document);
			return gs;
		}

		public static void SetActiveInstance(GameState gameState) {
			if ( IsActiveInstanceExists ) {
				Debug.LogError("Can't set active game state instance: another active instance already exists");
				return;
			}
			ActiveInstance = gameState;
		}

		public static void TryReleaseActiveInstance() {
			if ( !IsActiveInstanceExists ) {
				return;
			}
			ActiveInstance = null;
		}

		public static void TryRemoveSave(string profileName) {
			var di = new DirectoryInfo(XmlUtils.BasePath);
			if ( !di.Exists ) {
				return;
			}
			foreach ( var fi in di.EnumerateFiles("*.stpsave") ) {
				if ( Path.GetFileNameWithoutExtension(fi.Name) == profileName ) {
					fi.Delete();
				}
			}
		}
	}
}
