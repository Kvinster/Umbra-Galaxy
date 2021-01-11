using System;
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

		static void AddAttrValue<T>(this XmlElement element, string attrName, T value, Func<T, string> converter) {
			element.SetAttribute(attrName, converter(value));
		}
	}
}
