using System.Xml;

using STP.Utils.Xml;

namespace STP.Config {
    public sealed class DialogChoiceInfo : IXmlNodeLoadable {
        public BaseDialogText Text        { get; private set; }
        public string         ResponseKey { get; private set; }
        public string         Condition   { get; private set; }
        
        public void Load(XmlNode node) {
            Text        = DialogTextLoader.LoadDialogText(node);
            ResponseKey = node.GetAttrValue("response_key", string.Empty);
            Condition   = node.GetAttrValue("condition"   , string.Empty);
        }
    }
}
