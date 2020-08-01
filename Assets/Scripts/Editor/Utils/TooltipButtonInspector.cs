using UnityEditor;
using UnityEditor.UI;

using STP.Behaviour.Utils;

namespace STP.Editor.Utils {
    [CustomEditor(typeof(TooltipButton))]
    public sealed class TooltipButtonInspector : ButtonEditor {
        public override void OnInspectorGUI() {
            EditorGUILayout.LabelField("Default Button", EditorStyles.boldLabel);
            base.OnInspectorGUI();
            
            EditorGUILayout.LabelField("Tooltip Button", EditorStyles.boldLabel);
            var tooltipButton = target as TooltipButton;
            if ( !tooltipButton ) {
                return;
            }
            var tooltipProp = serializedObject.FindProperty("Tooltip");
            if ( tooltipProp != null ) {
                EditorGUILayout.PropertyField(tooltipProp);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
