using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

using STP.Behaviour.Meta;
using STP.Editor.Meta;
using STP.Utils.PropertyAttribute;

namespace STP.Editor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(ShardStarSystemIdAttribute))]
    public sealed class ShardStarSystemIdPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            StarSystemsGraphInfo graphInfo;
            
            var window = StarSystemsGraphEditorWindow.Instance;
            if ( window && (window.GraphInfo != null) ) {
                graphInfo = window.GraphInfo;
            } else {
                var graphInfoSO =
                    AssetDatabase.LoadAssetAtPath<StarSystemsGraphInfoScriptableObject>(
                        StarSystemsGraphInfoScriptableObject.FullAssetPath);
                if ( graphInfoSO ) {
                    graphInfo = graphInfoSO.StarSystemsGraphInfo;
                } else {
                    Debug.LogError("Can't load StarSystemsGraphInfoScriptableObject");
                    return;
                }
            }
            if ( graphInfo == null ) {
                Debug.LogError("Can't get StarSystemsGraphInfo");
                return;
            }
            var starSystemsIds  = new List<string>(graphInfo.ShardSystemsIds);
            var starSystemNames = new List<string>(starSystemsIds.Count);
            foreach ( var starSystemId in starSystemsIds ) {
                starSystemNames.Add(graphInfo.GetShardSystemName(starSystemId));
            }
            var curStarSystemId = property.stringValue;
            if ( starSystemsIds.Contains(curStarSystemId) || string.IsNullOrEmpty(curStarSystemId) ) {
                starSystemNames.Add("_Custom");
                starSystemNames.Add("_None");
                int index;
                if ( string.IsNullOrEmpty(curStarSystemId) ) {
                    index = starSystemNames.Count - 1;
                } else {
                    index = starSystemsIds.IndexOf(curStarSystemId);
                }
                index = EditorGUI.Popup(position, property.displayName, index, starSystemNames.ToArray());
                property.stringValue =
                    (starSystemNames[index] == "_None") ? string.Empty :
                    (starSystemNames[index] == "_Custom") ? "_Custom" : starSystemsIds[index];
            } else {
                property.stringValue = EditorGUI.TextField(position, property.displayName, curStarSystemId);
            }
        }
    }
}
