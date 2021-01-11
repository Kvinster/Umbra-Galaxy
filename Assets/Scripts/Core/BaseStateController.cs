using System.Xml;

namespace STP.Core {
	public abstract class BaseStateController {
		public abstract string Name { get; }

		public virtual void Load(XmlNode node) { }
		public virtual void Save(XmlElement elem) { }
		public virtual void Deinit() { }
	}
}
