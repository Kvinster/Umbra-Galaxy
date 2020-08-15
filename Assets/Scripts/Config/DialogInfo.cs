using System.Collections.Generic;
using System.Xml;

using STP.Utils.Xml;

namespace STP.Config {
    public sealed class DialogInfo : IXmlNodeLoadable {
        public string                 DialogName { get; private set; }
        public BaseDialogText         Text       { get; private set; }
        public List<DialogChoiceInfo> Choices    { get; private set; }
        
        public void Load(XmlNode node) {
            DialogName = node.GetAttrValue("name", string.Empty);
            Text       = DialogTextLoader.LoadDialogText(node.GetFirstChildByName("text"));
            Choices    = node.LoadNodeList("choices", "choice", () => new DialogChoiceInfo());
        }
    }
}
