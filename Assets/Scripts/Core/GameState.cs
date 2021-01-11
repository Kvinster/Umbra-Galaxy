using UnityEngine;

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

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
			Debug.Log(savePath);
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
			throw new NotImplementedException();
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
