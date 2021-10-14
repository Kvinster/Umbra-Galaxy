using System.Xml;

namespace STP.Core.State {
	public sealed class LevelControllerState : BaseState {
		public int CurLevelIndex = -1;

		public override string Name => "gameplay_progress";

		public override void Load(XmlNode node) {
			//Do nothing
		}

		public override void Save(XmlElement elem) {
			//Do nothing
		}
	}
}
