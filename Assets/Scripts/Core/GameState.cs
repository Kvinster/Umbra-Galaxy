using UnityEngine;

using System;
using System.Collections.Generic;

namespace STP.Core {
	public sealed class GameState {
		public static GameState Instance { get; private set; }

		public static bool IsInstanceExists => (Instance != null);

		readonly List<BaseStateController> _controllers = new List<BaseStateController>();

		public ChunkController  ChunkController  { get; }
		public LevelController  LevelController  { get; }
		public PlayerController PlayerController { get; }
		public XpController     XpController     { get; }

		GameState() {
			ChunkController  = AddController(new ChunkController());
			LevelController  = AddController(new LevelController());
			PlayerController = AddController(new PlayerController());
			XpController     = AddController(new XpController());
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

		public static GameState CreateNewGameState() {
			if ( IsInstanceExists ) {
				Debug.LogError("GameState instance already exists");
				return Instance;
			}
			Instance = new GameState();
			return Instance;
		}

		public static GameState LoadGameState(string gameStateSerialized) {
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
