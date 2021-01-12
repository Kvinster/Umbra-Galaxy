using System.Xml;

using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class XpState : BaseState {
		public int CurXp;

		public override string Name => "xp";

		public override void Load(XmlNode node) {
			CurXp = node.GetAttrValue("xp", 0);
		}

		public override void Save(XmlElement elem) {
			elem.AddAttrValue("xp", CurXp);
		}
	}
}
