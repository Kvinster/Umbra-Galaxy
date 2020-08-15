using System.Xml;

using STP.Utils.Xml;

namespace STP.Config {
    public abstract class BaseConfig : IXmlNodeLoadable {
        public abstract void Load(XmlNode root);
    }
}
