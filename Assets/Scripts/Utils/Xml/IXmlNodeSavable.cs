using System.Xml;

namespace STP.Utils.Xml {
	public interface IXmlNodeSavable {
		void Save(XmlElement elem);
	}
}
