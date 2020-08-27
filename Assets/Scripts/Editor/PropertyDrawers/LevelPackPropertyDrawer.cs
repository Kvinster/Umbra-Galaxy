using UnityEditor;
using UnityEngine;

using STP.Config.ScriptableObjects;

namespace STP.Editor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(CoreLevelsCatalogue.LevelPack))]
    public class LevelPackPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var type   = typeof(CoreLevelsCatalogue.LevelPack);
            var fields = type.GetFields();
            var levelTypeProp = property.FindPropertyRelative(fields[0].Name);
            var levelsProp    = property.FindPropertyRelative(fields[1].Name);
            var yStart = position.y;
            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, yStart, position.width, 16), property.isExpanded, ((LevelType)levelTypeProp.intValue).ToString(), true);
            yStart += 16;
            if ( !property.isExpanded ) {
                return;
            }
            var levelTypePropHeight = EditorGUI.GetPropertyHeight(levelTypeProp);
            var levelPropHeight     = EditorGUI.GetPropertyHeight(levelsProp);
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(new Rect(position.x, yStart, position.width, levelTypePropHeight), levelTypeProp, true);
            yStart += levelTypePropHeight;
            EditorGUI.PropertyField(new Rect(position.x, yStart, position.width, levelPropHeight), levelsProp, true);
            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var type   = typeof(CoreLevelsCatalogue.LevelPack);
            var fields = type.GetFields();
            var levelTypeProp = property.FindPropertyRelative(fields[0].Name);
            var levelsProp    = property.FindPropertyRelative(fields[1].Name);
            
            var levelTypePropHeight = EditorGUI.GetPropertyHeight(levelTypeProp);
            var levelPropHeight     = EditorGUI.GetPropertyHeight(levelsProp);
            return ((property.isExpanded) ? levelTypePropHeight + levelPropHeight : 0) + 16;
        }
    }
}