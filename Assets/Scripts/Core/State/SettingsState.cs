using System.Xml;

using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class SettingsState : BaseState {
		public float MasterVolume = -30f;

		public override string Name => "settings";

		public override void Load(XmlNode node) {
			MasterVolume = node.GetAttrValue("master_volume", -30f);
		}

		public override void Save(XmlElement elem) {
			elem.AddAttrValue("master_volume", MasterVolume);
		}
	}
}
