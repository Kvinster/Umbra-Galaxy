using UnityEngine;

using System.Collections.Generic;
using System.IO;
using System.Xml;

using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class ProfileState {
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

		ProfileState(string stateName) {
			StateName   = stateName;
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

		public static ProfileState CreateNewProfileState(string stateName, string profileName) {
			var profileState = new ProfileState(stateName) { ProfileName = profileName };
			return profileState;
		}

		public static ProfileState LoadGameState(string stateName) {
			var document = XmlUtils.LoadGameStateDocument(stateName);
			if ( document == null ) {
				Debug.LogErrorFormat("Can't load save for state name '{0}'", stateName);
				return null;
			}
			var gs = new ProfileState(stateName);
			gs.Load(document);
			return gs;
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
