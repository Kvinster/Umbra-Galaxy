using System.Xml;

using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class LevelState : BaseState {
		public int LastLevelIndex = -1;

		public int CurLevelIndex { get; set; } = -1; // don't save!

		public override string Name => "level";

		public LevelState() {
			TryFixNextLevelIndex();
		}

		public override void Load(XmlNode node) {
			LastLevelIndex = node.GetAttrValue("next_level", -1);
			TryFixNextLevelIndex();
		}

		public override void Save(XmlElement elem) {
			elem.AddAttrValue("next_level", LastLevelIndex);
		}

		void TryFixNextLevelIndex() {
			if ( LastLevelIndex == -1 ) {
				LastLevelIndex = 0;
			}
		}
	}
}
