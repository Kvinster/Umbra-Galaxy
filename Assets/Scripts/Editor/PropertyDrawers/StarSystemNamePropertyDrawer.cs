using UnityEditor;
using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Meta;
using STP.Editor.Meta;
using STP.Utils.PropertyAttribute;

namespace STP.Editor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(StarSystemNameAttribute))]
    public class StarSystemNamePropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            List<string> starSystems;
            var window = StarSystemsGraphEditorWindow.Instance;
            if ( window ) {
                starSystems = window.StarSystems;
            } else {
                var graphInfoSO =
                    AssetDatabase.LoadAssetAtPath<StarSystemsGraphInfoScriptableObject>(
                        StarSystemsGraphInfoScriptableObject.FullAssetPath);
                if ( graphInfoSO ) {
                    starSystems = graphInfoSO.StarSystemsGraphInfo.StarSystems;
                } else {
                    Debug.LogError("Can't load StarSystemsGraphInfoScriptableObject");
                    return;
                }
            }
            var curStarSystem = property.stringValue;
            if ( starSystems.Contains(curStarSystem) || string.IsNullOrEmpty(curStarSystem) ) {
                starSystems.Add("_Custom");
                starSystems.Add("_None");
                int index;
                if ( string.IsNullOrEmpty(curStarSystem) ) {
                    index = starSystems.Count - 1;
                } else {
                    index = starSystems.IndexOf(curStarSystem);
                }
                index = EditorGUI.Popup(position, property.displayName, index, starSystems.ToArray());
                property.stringValue = (starSystems[index] == "_None") ? string.Empty : starSystems[index];
            } else {
                property.stringValue = EditorGUI.TextField(position, property.displayName, curStarSystem);
            }
        }
    }
}
