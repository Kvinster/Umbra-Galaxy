using System.Xml;

using STP.Utils.Xml;

namespace STP.Core.State {
	public abstract class BaseState : IXmlNodeSerializable {
		public abstract string Name { get; }

		public abstract void Load(XmlNode node);
		public abstract void Save(XmlElement elem);
	}
}
