using UnityEngine;

using System.Xml;

using STP.Utils.Xml;

namespace STP.Config {
    public static class DialogTextLoader {
        public static BaseDialogText LoadDialogText(XmlNode node) {
            Debug.Assert(node != null, "node != null");
            var singleText = node.GetAttrValue("text", string.Empty);
            if ( !string.IsNullOrEmpty(singleText) ) {
                return new SingleDialogText(singleText);
            }
            var multipleTexts = node.LoadNodeList("text", "text");
            if ( multipleTexts.Count > 0 ) {
                return new RandomDialogText(multipleTexts);
            }
            Debug.LogErrorFormat("Node '{0}' has neither single no multiple text options", node.Name);
            return null;
        }
    }
}
