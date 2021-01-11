using System.Xml;

using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class PlayerControllerState : IXmlNodeSerializable {
		public int CurLives;

		public void Load(XmlNode node) {
			CurLives = node.GetAttrValue("lives", 0);
		}

		public void Save(XmlElement elem) {
			elem.AddAttrValue("lives", CurLives);
		}
	}
}
