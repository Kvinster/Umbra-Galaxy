using System.Xml;

using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class LevelState : BaseState {
		public int NextLevelIndex = -1;

		public override string Name => "level";

		public LevelState() {
			TryFixNextLevelIndex();
		}

		public override void Load(XmlNode node) {
			NextLevelIndex = node.GetAttrValue("next_level", -1);
			TryFixNextLevelIndex();
		}

		public override void Save(XmlElement elem) {
			elem.AddAttrValue("next_level", NextLevelIndex);
		}

		void TryFixNextLevelIndex() {
			if ( NextLevelIndex == -1 ) {
				NextLevelIndex = 0;
			}
		}
	}
}
