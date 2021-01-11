using System.Xml;

using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class XpControllerState : IXmlNodeSerializable {
		public int CurXp;

		public void Load(XmlNode node) {
			CurXp = node.GetAttrValue("xp", 0);
		}

		public void Save(XmlElement elem) {
			elem.AddAttrValue("xp", CurXp);
		}
	}
}
