using UnityEngine;

namespace STP.View.DebugGUI {
	public static class DebugGuiUtils {
		static float    _sizeCoef       = -1;
		static GUIStyle _buttonStyle    = null;
		static GUIStyle _textBlockStyle = null;

		static float SizeCoef {
			get {
				if ( _sizeCoef < 0 ) {
					var sw = Screen.width;
					var sh = Screen.height;
					_sizeCoef = Mathf.Min(sw, sh);
				}

				return _sizeCoef;
			}
		}

		static float RelativeSize(float value) {
			return (value * SizeCoef) / 12;
		}
	
		static GUIStyle ButtonStyle {
			get {
				if ( _buttonStyle != null ) {
					return _buttonStyle;
				}

				_buttonStyle = new GUIStyle(GUI.skin.button) {fontSize = (int)RelativeSize(0.3f)};
				return _buttonStyle;
			}
		}
		
		static GUIStyle TextBlockStyle {
			get {
				if ( _textBlockStyle != null ) {
					return _buttonStyle;
				}

				_textBlockStyle = new GUIStyle(GUI.skin.textArea) {fontSize = (int)RelativeSize(1f)};
				return _textBlockStyle;
			}
		}

		public static void BeginVertical() {
			GUILayout.BeginVertical();
		}

		public static void EndVertical() {
			GUILayout.EndVertical();
		}

		public static void BeginArea(Rect rect) {
			GUILayout.BeginArea(new Rect(RelativeSize(rect.x), RelativeSize(rect.y), RelativeSize(rect.width), RelativeSize(rect.height)));
		}

		public static void EndArea() => GUILayout.EndArea();

		public static string TextFieldWithButton(string fieldText, string buttonText, out bool buttonPressed) {
			GUILayout.BeginHorizontal();
			var style = new GUIStyle(GUI.skin.GetStyle("textarea")) {
				alignment = TextAnchor.MiddleCenter
			};
			fieldText     = GUILayout.TextField(fieldText, style, GUILayout.Height(RelativeSize(1)), GUILayout.ExpandWidth(true));
			buttonPressed = Button(buttonText);
			GUILayout.EndHorizontal();
			return fieldText;
		}

		public static bool Button(string text) {
			return GUILayout.Button(text, ButtonStyle, GUILayout.Height(RelativeSize(1)), GUILayout.ExpandWidth(true));
		}

		public static void TextArea(string text, int relativeH = 1, int relativeW = 1) {
			GUILayout.TextArea(text, TextBlockStyle, GUILayout.Height(RelativeSize(relativeH)), GUILayout.Width(RelativeSize(relativeW)));
		}
	}
}

//