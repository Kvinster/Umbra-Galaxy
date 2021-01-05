﻿using UnityEngine;

using STP.Behaviour.Core;
using STP.Behaviour.Core.LevelGeneration;
using STP.Controller;
using STP.Manager;
using STP.Utils.GameComponentAttributes;
using STP.View.DebugGUI;

namespace STP.Behaviour.Starter {
	public class CoreStarter : BaseStarter<CoreStarter> {
		[NotNull] public Player             Player;
		[NotNull] public Transform          PlayerStartPos;
		[NotNull] public LevelGenerator     Generator;
		[NotNull] public CoreWindowsManager CoreWindowsManager;

		public PlayerManager    PlayerManager    { get; private set; }
		public LevelGoalManager LevelGoalManager { get; private set; }

		void Start() {
			var pc = PlayerController.Instance;
			var lc = LevelController.Instance;
			PlayerManager    = new PlayerManager(Player, pc);
			LevelGoalManager = new LevelGoalManager(Player.transform, lc);
			CoreWindowsManager.Init(PlayerManager, LevelGoalManager, pc);
			Generator.Init(lc, ChunkController.Instance);
			Generator.GenerateLevel();
			InitComponents();
			// Settings for smooth gameplay
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
			QualitySettings.vSyncCount  = 0;
		}

		void OnDestroy() {
			if ( DebugGuiController.HasInstance ) {
				DebugGuiController.Instance.SetDrawable(null);
			}
		}
	}
}
