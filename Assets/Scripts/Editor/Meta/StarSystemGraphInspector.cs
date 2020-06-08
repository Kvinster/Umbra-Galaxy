using UnityEditor;
using UnityEngine;

using STP.Behaviour.Meta;

namespace STP.Editor.Meta {
    [CustomEditor(typeof(StarSystemsGraphInfo))]
    public class StarSystemGraphInspector : UnityEditor.Editor {
        static readonly Color SeparatorLineColor = new Color(0.5f,0.5f,0.5f, 1);
        
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
            var list = graphInfo.GetStarSystemPairsInEditor();
            for ( var i = 0; i < list.Count; i++ ) {
                var pair = list[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label($"'{pair.A}' x '{pair.B}':", GUILayout.Width(width));
                GUILayout.Label(pair.Distance.ToString(), GUILayout.Width(width));
                GUILayout.EndHorizontal();
                if ( i != list.Count - 1 ) {
                    DrawLine(1);
                }
            }
            GUILayout.Space(10);
            DrawLine(3);
            GUILayout.Space(10);
            width = Screen.width / 3f;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Star System", EditorStyles.boldLabel, GUILayout.Width(width));
            GUILayout.Label("Faction", EditorStyles.boldLabel, GUILayout.Width(width));
            GUILayout.Label("Start money", EditorStyles.boldLabel, GUILayout.Width(width));
            GUILayout.EndHorizontal();
            foreach ( var startInfo in graphInfo.GetStarSystemStartInfosInEditor() ) {
                GUILayout.BeginHorizontal();
                GUILayout.Label(startInfo.Name, GUILayout.Width(width));
                GUILayout.Label(startInfo.Faction.ToString(), GUILayout.Width(width));
                GUILayout.Label(startInfo.StartMoney.ToString(), GUILayout.Width(width));
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(10);
            DrawLine(3);
            GUILayout.Space(10);
        }

        void DrawLine(float height) {
            var rect = EditorGUILayout.GetControlRect(false, height);
            EditorGUI.DrawRect(rect, SeparatorLineColor);
        }
    }
}
