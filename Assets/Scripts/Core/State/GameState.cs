using UnityEngine;

using System.Collections.Generic;
using System.IO;
using System.Xml;

using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class GameState {
		public static GameState ActiveInstance { get; private set; }

		public static bool IsActiveInstanceExists => (ActiveInstance != null);

		public readonly string StateName;

		readonly List<BaseState> _states = new List<BaseState>();

		public CommonState CommonState { get; }
		public LevelState  LevelState  { get; }

		public string ProfileName {
			get => CommonState.ProfileName;
			private set {
				CommonState.ProfileName = value;
				Save();
			}
		}

		GameState(string stateName) {
			StateName = stateName;

			CommonState = AddState(new CommonState());
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
			document.SaveGameStateDocument(StateName);
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

		public static GameState CreateNewActiveGameState(string stateName, string profileName) {
			if ( IsActiveInstanceExists ) {
				Debug.LogError("GameState active instance already exists");
				return ActiveInstance;
			}
			ActiveInstance             = new GameState(stateName);
			ActiveInstance.ProfileName = profileName;
			return ActiveInstance;
		}

		public static GameState LoadGameState(string stateName) {
			var document = XmlUtils.LoadGameStateDocument(stateName);
			if ( document == null ) {
				Debug.LogErrorFormat("Can't load save for state name '{0}'", stateName);
				return null;
			}
			var gs = new GameState(stateName);
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
			ReleaseActiveInstance();
		}

		public static void ReleaseActiveInstance() {
			if ( !IsActiveInstanceExists ) {
				Debug.LogError("No active release instance");
				return;
			}
			ActiveInstance = null;
		}

		public static bool TryRemoveSave(string stateName) {
			var di = new DirectoryInfo(XmlUtils.BasePath);
			if ( !di.Exists ) {
				return false;
			}
			var success = false;
			foreach ( var fi in di.EnumerateFiles("*.stpsave") ) {
				if ( Path.GetFileNameWithoutExtension(fi.Name) == stateName ) {
					success = true;
					fi.Delete();
				}
			}
			return success;
		}
	}
}
