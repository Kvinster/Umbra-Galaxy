using System.Collections.Generic;
using System.Xml;
using STP.Config;
using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class LevelControllerState : BaseState {
		public class CompletedMainLevelInfo {
			public LevelNode MainLevel;
			
			readonly List<LevelNode> _completedOptionalLevels = new List<LevelNode>();

			public bool IsNextMainLevel(LevelNode node) {
				return MainLevel.NextLevels.Contains(node);
			}

			public bool IsOptionalLevel(LevelNode node) {
				return MainLevel.OptionalLevels.Contains(node);
			}
			
			public bool IsOptionalLevelCompleted(LevelNode node) {
				return _completedOptionalLevels.Contains(node);
			}

			public void SetOptionalLevelAsCompleted(LevelNode node) {
				_completedOptionalLevels.Add(node);
			} 
		}
		
		public List<CompletedMainLevelInfo> CompletedLevels { get; private set; } = new List<CompletedMainLevelInfo>();

		public LevelNode CurStartedLevel;

		public override string Name => "gameplay_progress";

		public override void Load(XmlNode node) {
			//Do nothing
		}

		public override void Save(XmlElement elem) {
			//Do nothing
		}
	}
}
