using UnityEngine;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace STP.Utils.Xml {
	public static class XmlElementExtensions {
		static readonly Func<string, string> StringConverter = x => x;
		static readonly Func<int, string>    IntConverter    = x => x.ToString(CultureInfo.InvariantCulture);
		static readonly Func<bool, string>   BoolConverter   = x => x.ToString(CultureInfo.InvariantCulture);
		static readonly Func<float, string>  FloatConverter  = x => x.ToString(CultureInfo.InvariantCulture);

		public static void AddAttrValue(this XmlElement element, string attrName, string value) {
			element.AddAttrValue(attrName, value, StringConverter);
		}

		public static void AddAttrValue(this XmlElement element, string attrName, int value) {
			element.AddAttrValue(attrName, value, IntConverter);
		}

		public static void AddAttrValue(this XmlElement element, string attrName, bool value) {
			element.AddAttrValue(attrName, value, BoolConverter);
		}

		public static void AddAttrValue(this XmlElement element, string attrName, float value) {
			element.AddAttrValue(attrName, value, FloatConverter);
		}

		public static void SaveNodeList<T>(this XmlElement element, string parentName, string name, List<T> values)
			where T : IXmlNodeSavable {
			if ( values == null ) {
				Debug.LogError("List is null");
				return;
			}
			if ( string.IsNullOrEmpty(parentName) ) {
				Debug.LogError("Parent name is null or empty");
				return;
			}
			if ( string.IsNullOrEmpty(name) ) {
				Debug.LogError("Child name is null or empty");
				return;
			}
			var parentNode = element.SelectSingleNode(parentName);
			if ( parentNode == null ) {
				parentNode = element.OwnerDocument.CreateElement(parentName);
				element.AppendChild(parentNode);
			}
			foreach ( var value in values ) {
				if ( value == null ) {
					Debug.LogError("Value is null");
					continue;
				}
				var childNode = parentNode.OwnerDocument.CreateElement(name);
				value.Save(childNode);
				parentNode.AppendChild(childNode);
			}
		}

		static void AddAttrValue<T>(this XmlElement element, string attrName, T value, Func<T, string> converter) {
			element.SetAttribute(attrName, converter(value));
		}
	}
}
