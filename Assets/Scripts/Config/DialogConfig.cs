using System.Collections.Generic;
using System.Xml;

using STP.Utils.Xml;

namespace STP.Config {
    public sealed class DialogConfig : BaseConfig {
        public List<DialogInfo> Dialogs { get; private set; }
        
        public override void Load(XmlNode root) {
            Dialogs = root.LoadNodeList("dialog", () => new DialogInfo());
        }
    }
}
