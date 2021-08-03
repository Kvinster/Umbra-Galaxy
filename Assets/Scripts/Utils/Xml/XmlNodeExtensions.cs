using UnityEngine;

using System;
using System.Collections.Generic;
using System.Globalization;
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

        static readonly Func<string, string> StringConverter = x => x;
        static readonly Func<string, int>    IntConverter    = int.Parse;
        static readonly Func<string, bool>   BoolConverter   = bool.Parse;
        static readonly Func<string, float>  FloatConverter  = (x) => float.Parse(x, CultureInfo.InvariantCulture);

        public static string GetAttrValue(this XmlNode node, string attrName, string def) {
            return node.TryLoadAttrValue(attrName, StringConverter, out var value) ? value : def;
        }

        public static int GetAttrValue(this XmlNode node, string attrName, int def) {
            return node.TryLoadAttrValue(attrName, IntConverter, out var value) ? value : def;
        }

        public static bool GetAttrValue(this XmlNode node, string attrName, bool def) {
            return node.TryLoadAttrValue(attrName, BoolConverter, out var value) ? value : def;
        }

        public static float GetAttrValue(this XmlNode node, string attrName, float def) {
            return node.TryLoadAttrValue(attrName, FloatConverter, out var value) ? value : def;
        }

        public static T GetAttrValue<T>(this XmlNode node, string attrName, T def) where T : Enum {
            return node.TryLoadAttrValue(attrName, x => (T)Enum.Parse(typeof(T), x), out var value) ? value : def;
        }

        public static List<string> LoadNodeList(this XmlNode node, string nodeName, string attrName,
            string def = default) {
            return node.LoadNodeList(nodeName, attrName, StringConverter, def);
        }

        public static List<int> LoadNodeList(this XmlNode node, string nodeName, string attrName, int def = default) {
            return node.LoadNodeList(nodeName, attrName, IntConverter, def);
        }

        public static List<bool> LoadNodeList(this XmlNode node, string nodeName, string attrName, bool def = default) {
            return node.LoadNodeList(nodeName, attrName, BoolConverter, def);
        }

        public static List<float> LoadNodeList(this XmlNode node, string nodeName, string attrName,
            float def = default) {
            return node.LoadNodeList(nodeName, attrName, FloatConverter, def);
        }

        public static List<T> LoadNodeList<T>(this XmlNode node, string parentName, string nodeName, Func<T> init)
            where T : IXmlNodeLoadable {
            Debug.Assert(init != null, "init != null");
            var parentNode = node.GetFirstChildByName(parentName);
            return parentNode?.LoadNodeList(nodeName, init);
        }

        public static XmlNode FindChild(this XmlNode node, string childName) {
            XmlNode child = null;
            node.ForEachChild(x => {
                if ( x.Name == childName ) {
                    child = x;
                }
            });
            return child;
        }

        static List<T> LoadNodeList<T>(this XmlNode node, string nodeName, Func<T> init)
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

        static List<T> LoadNodeList<T>(this XmlNode node, string nodeName, string attrName, Func<string, T> converter,
            T def) {
            var res = new List<T>();
            node.ForEachChild(x => {
                if ( x.Name != nodeName ) {
                    return;
                }
                res.Add(x.TryLoadAttrValue(attrName, converter, out var value) ? value : def);
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

        static bool TryLoadAttrValue<T>(this XmlNode node, string attrName, Func<string, T> converter, out T value) {
            if ( TryLoadAttrValueRaw(node, attrName, out var valueRaw) ) {
                value = converter(valueRaw);
                return true;
            }
            value = default;
            return false;
        }

        static bool TryLoadAttrValueRaw(this XmlNode node, string attrName, out string value) {
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
