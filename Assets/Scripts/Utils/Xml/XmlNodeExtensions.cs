using UnityEngine;

using System;
using System.Collections.Generic;
using System.Xml;

namespace STP.Utils.Xml {
    public static class XmlNodeExtensions {
        public static XmlNode GetFirstChildByName(this XmlNode node, string childNodeName) {
            for ( var i = 0; i < node.ChildNodes.Count; ++i ) {
                var childNode = node.ChildNodes[i];
                if ( childNode.Name == childNodeName ) {
                    return childNode;
                }
            }
            return null;
        }
        
        public static string GetAttrValue(this XmlNode node, string attrName, string def) {
            return node.TryLoadAttrValue(attrName, out var value) ? value : def;
        }

        public static List<string> LoadNodeList(this XmlNode node, string nodeName, string attrName) {
            var res = new List<string>();
            node.ForEachChild(x => {
                if ( x.Name != nodeName ) {
                    return;
                }
                if ( x.TryLoadAttrValue(attrName, out var value) ) {
                    res.Add(value);
                }
            });
            res.TrimExcess();
            return res;
        }
        
        public static List<T> LoadNodeList<T>(this XmlNode node, string parentName, string nodeName, Func<T> init)
            where T : IXmlNodeLoadable {
            Debug.Assert(init != null, "init != null");
            var parentNode = node.GetFirstChildByName(parentName);
            return parentNode?.LoadNodeList(nodeName, init);
        }
        
        public static List<T> LoadNodeList<T>(this XmlNode node, string nodeName, Func<T> init)
            where T : IXmlNodeLoadable {
            Debug.Assert(init != null, "init != null");
            var res = new List<T>();
            node.ForEachChild(x => {
                if ( x.Name != nodeName ) {
                    return;
                }
                var loadable = init.Invoke();
                loadable.Load(x);
                res.Add(loadable);
            });
            res.TrimExcess();
            return res;
        }

        static void ForEachChild(this XmlNode node, Action<XmlNode> action) {
            Debug.Assert(action != null, "action != null");
            for ( var i = 0; i < node.ChildNodes.Count; ++i ) {
                var childNode = node.ChildNodes[i];
                action.Invoke(childNode);
            }
        }

        static bool TryLoadAttrValue(this XmlNode node, string attrName, out string value) {
            value = null;
            if ( node.Attributes == null ) {
                return false;
            }
            for ( var i = 0; i < node.Attributes.Count; ++i ) {
                var attr = node.Attributes[i];
                if ( attr.Name == attrName ) {
                    value = attr.Value;
                    return true;
                }
            }
            return false;
        }
    }
}
