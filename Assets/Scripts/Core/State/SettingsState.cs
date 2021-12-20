using System.Xml;

using STP.Utils.Xml;

namespace STP.Core.State {
	public sealed class SettingsState : BaseState {
		public float MasterVolume = -20f;
		public float MusicVolume;
		public float SfxVolume;

		public override string Name => "settings";

		public override void Load(XmlNode node) {
			MasterVolume = node.GetAttrValue("master_volume", -20f);
			MusicVolume  = node.GetAttrValue("music_volume", 0f);
			SfxVolume    = node.GetAttrValue("sfx_volume", 0f);
		}

		public override void Save(XmlElement elem) {
			elem.AddAttrValue("master_volume", MasterVolume);
			elem.AddAttrValue("music_volume", MusicVolume);
			elem.AddAttrValue("sfx_volume", SfxVolume);
		}
	}
}
