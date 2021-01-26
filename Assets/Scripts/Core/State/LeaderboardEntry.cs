using System.Xml;

using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class LeaderboardEntry : IXmlNodeSerializable {
		public string ProfileName;
		public int    Highscore;

		public void Load(XmlNode node) {
			ProfileName = node.GetAttrValue("name", string.Empty);
			Highscore   = node.GetAttrValue("score", 0);
		}

		public void Save(XmlElement elem) {
			elem.AddAttrValue("name", ProfileName);
			elem.AddAttrValue("score", Highscore);
		}
	}
}
