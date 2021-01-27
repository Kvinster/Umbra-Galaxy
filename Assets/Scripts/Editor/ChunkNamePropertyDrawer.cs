using UnityEngine;
using UnityEditor;

using System.Linq;

using STP.Config;
using STP.Utils.Attributes;

namespace STP.Editor {
	[CustomPropertyDrawer(typeof(ChunkNameAttribute))]
	public class ChunkNamePropertyDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var chunkConfig   = Resources.Load<ChunkConfig>("ChunkConfig");
			var allChunkNames = chunkConfig.ChunkInfos.Select(x => x.Name).ToList();
			var index = EditorGUI.Popup(position, Mathf.Max(0, allChunkNames.IndexOf(property.stringValue)), allChunkNames.ToArray());
			property.stringValue = allChunkNames[index];
		}
	}
}