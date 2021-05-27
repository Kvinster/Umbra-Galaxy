using UnityEngine;

using System.Collections.Generic;
using System.Xml;

using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class GameState {
		public static GameState ActiveInstance { get; private set; }

		public static bool IsActiveInstanceExists => (ActiveInstance != null);

		public static string StateName => "common_state";


		readonly List<BaseState> _states = new List<BaseState>();

		public SettingsState        SettingsState        { get; }
		public LeaderboardState     LeaderboardState     { get; }
		public LevelControllerState LevelControllerState { get; }

		GameState() {
			LevelControllerState = AddState(new LevelControllerState());
			LeaderboardState     = AddState(new LeaderboardState());
			SettingsState        = AddState(new SettingsState());
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

		public static GameState CreateNewActiveGameState() {
			if ( IsActiveInstanceExists ) {
				Debug.LogError("GameState active instance already exists");
				return ActiveInstance;
			}

			ActiveInstance = new GameState();
			ActiveInstance.Save();
			return ActiveInstance;
		}

		public static GameState TryLoadActiveGameState() {
			if ( IsActiveInstanceExists ) {
				Debug.LogError("GameState active instance already exists");
				return ActiveInstance;
			}

			if ( !XmlUtils.IsGameStateDocumentExists(StateName) ) {
				return null;
			}

			var document = XmlUtils.LoadGameStateDocument(StateName);
			if ( document == null ) {
				Debug.LogErrorFormat("Can't load save for state name '{0}'", StateName);
				return null;
			}

			ActiveInstance = new GameState();
			ActiveInstance.Load(document);
			return ActiveInstance;
		}

		public static void ReleaseActiveInstance() {
			if ( !IsActiveInstanceExists ) {
				Debug.LogError("No active release instance");
				return;
			}

			ActiveInstance = null;
		}
	}
}