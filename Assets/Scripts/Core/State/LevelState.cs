﻿using UnityEngine;

using System.Xml;

using STP.Config;
using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class LevelState : BaseState {
		public string NextLevelName;

		public override string Name => "level";

		public override void Load(XmlNode node) {
			NextLevelName = node.GetAttrValue("next_level", string.Empty);
			// TODO: maybe do this somewhere else
			if ( string.IsNullOrEmpty(NextLevelName) ) {
				var levelsConfig = Resources.Load<LevelsConfig>("AllLevels");
				if ( levelsConfig ) {
					NextLevelName = levelsConfig.Levels[0].LevelName;
				} else {
					Debug.LogError("Can't load LevelsConfig");
				}
			}
		}

		public override void Save(XmlElement elem) {
			elem.AddAttrValue("next_level", NextLevelName);
		}
	}
}