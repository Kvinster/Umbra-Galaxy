using System.Xml;

using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class PlayerState : BaseState {
		public const int StartPlayerLives = 3;

		public int CurLives = StartPlayerLives;

		public override string Name => "player";

		public override void Load(XmlNode node) {
			CurLives = node.GetAttrValue("lives", 0);
		}

		public override void Save(XmlElement elem) {
			elem.AddAttrValue("lives", CurLives);
		}
	}
}
