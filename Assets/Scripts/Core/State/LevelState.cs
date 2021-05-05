using System.Xml;

namespace STP.Core.State {
	public sealed class LevelState : BaseState {
		public int LastLevelIndex;

		public int CurLevelIndex { get; set; } = -1;

		public override string Name => "level";

		public override void Load(XmlNode node) {
			// Do nothing
		}

		public override void Save(XmlElement elem) {
			// Do nothing
		}

		public void ResetState() {
			LastLevelIndex = 0;
			CurLevelIndex = -1;
		}
	}
}
