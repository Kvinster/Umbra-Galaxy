using UnityEditor;
using UnityEngine;

using STP.Behaviour.Meta;

namespace STP.Editor.Meta {
    [CustomEditor(typeof(StarSystemsGraphInfo))]
    public class StarSystemGraphInspector : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            var graphInfo = target as StarSystemsGraphInfo;
            if ( !graphInfo ) {
                return;
            }
            if ( GUILayout.Button("Open editor window") ) {
                StarSystemsGraphEditorWindow.Init();
                var window =
                    (StarSystemsGraphEditorWindow) EditorWindow.GetWindow(typeof(StarSystemsGraphEditorWindow));
                window.SetGraph(graphInfo);
            }
            GUILayout.Space(10);
            var width = Screen.width / 2f;
            foreach ( var pair in graphInfo.GetStarSystemPairsInEditor() ) {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"'{pair.A}' x '{pair.B}':", GUILayout.Width(width));
                GUILayout.Label(pair.Distance.ToString(), GUILayout.Width(width));
                GUILayout.EndHorizontal();
                Rect rect = EditorGUILayout.GetControlRect(false, 1);
                rect.height = 1;
                EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
            }
        }
    }
}
