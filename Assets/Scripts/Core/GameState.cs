using UnityEngine;

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using STP.Utils.Xml;

namespace STP.Core {
	public sealed class GameState {
		public static GameState Instance { get; private set; }

		public static bool IsInstanceExists => (Instance != null);

		readonly string _profileName;

		readonly List<BaseStateController> _controllers = new List<BaseStateController>();

		public ChunkController  ChunkController  { get; }
		public LevelController  LevelController  { get; }
		public PlayerController PlayerController { get; }
		public XpController     XpController     { get; }

		GameState(string profileName) {
			_profileName = profileName;

			ChunkController  = AddController(new ChunkController());
			LevelController  = AddController(new LevelController());
			PlayerController = AddController(new PlayerController());
			XpController     = AddController(new XpController());
		}

		public void Save() {
			var di = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "saves"));
			if ( !di.Exists ) {
				di.Create();
			}
			var savePath = Path.Combine(di.ToString(), $"{_profileName}.xml");
			Debug.LogFormat("Saving to '{0}'", savePath);
			var fi = new FileInfo(savePath);
			if ( fi.Exists ) {
				fi.Delete();
			}
			var document = new XmlDocument();
			var root     = document.CreateElement("root");
			document.AppendChild(root);
			foreach ( var controller in _controllers ) {
				var childElement = document.CreateElement(controller.Name);
				controller.Save(childElement);
				root.AppendChild(childElement);
			}
			document.Save(savePath);
		}

		void Deinit() {
			foreach ( var controller in _controllers ) {
				controller.Deinit();
			}
		}

		void Load(XmlDocument xmlDocument) {
			var root = xmlDocument.DocumentElement;
			foreach ( var controller in _controllers ) {
				var childNode = root.FindChild(controller.Name);
				if ( childNode != null ) {
					controller.Load(childNode);
				}
			}
		}

		T AddController<T>(T controller) where T : BaseStateController {
			_controllers.Add(controller);
			return controller;
		}

		public static GameState CreateNewGameState(string profileName) {
			if ( IsInstanceExists ) {
				Debug.LogError("GameState instance already exists");
				return Instance;
			}
			Instance = new GameState(profileName);
			return Instance;
		}

		public static GameState LoadGameState(string profileName) {
			var di = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "saves"));
			if ( !di.Exists ) {
				Debug.LogError("Saves directory does not exist");
				return null;
			}
			var loadPath = Path.Combine(di.ToString(), $"{profileName}.xml");
			Debug.LogFormat("Loading from: '{0}'", loadPath);
			var fi = new FileInfo(loadPath);
			if ( !fi.Exists ) {
				Debug.LogErrorFormat("Save file for '{0}' does not exist", profileName);
				return null;
			}
			var document = new XmlDocument();
			document.Load(loadPath);
			Instance = new GameState(profileName);
			Instance.Load(document);
			return Instance;
		}

		public static void TryReleaseGameStateInstance() {
			if ( !IsInstanceExists ) {
				return;
			}
			Instance.Deinit();
			Instance = null;
		}
	}
}
