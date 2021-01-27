using System.Xml;

using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class CommonState : BaseState {
		public string ProfileName;

		public override string Name => "common";

		public override void Load(XmlNode node) {
			ProfileName = node.GetAttrValue("name", string.Empty);
		}

		public override void Save(XmlElement elem) {
			elem.AddAttrValue("name", ProfileName);
		}
	}
}
