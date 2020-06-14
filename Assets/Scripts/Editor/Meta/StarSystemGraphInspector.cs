using UnityEditor;
using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Meta;

namespace STP.Editor.Meta {
    [CustomEditor(typeof(StarSystemsGraphInfo))]
    public class StarSystemGraphInspector : UnityEditor.Editor {
        static readonly Color SeparatorLineColor = new Color(0.5f,0.5f,0.5f, 1);

        (string a, string b) _selectedPair = default;

        void OnEnable() {
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        void OnDisable() {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }

        public override void OnInspectorGUI() {
            var graphInfo = target as StarSystemsGraphInfo;
            if ( !graphInfo ) {
                return;
            }
            if ( !StarSystemsGraphEditorWindow.Instance ) {
                if ( GUILayout.Button("Open editor window") ) {
                    StarSystemsGraphEditorWindow.Init();
                    var window =
                        (StarSystemsGraphEditorWindow) EditorWindow.GetWindow(typeof(StarSystemsGraphEditorWindow));
                    window.SetGraph(graphInfo);
                }
            } else {
                DrawSelectedPairInspector();
            }
            // GUILayout.Space(10);
            // var width = Screen.width / 2f;
            // var list = graphInfo.GetStarSystemPairsInEditor();
            // for ( var i = 0; i < list.Count; i++ ) {
            //     var pair = list[i];
            //     GUILayout.BeginHorizontal();
            //     GUILayout.Label($"'{pair.A}' x '{pair.B}':", GUILayout.Width(width));
            //     GUILayout.Label(pair.Distance.ToString(), GUILayout.Width(width));
            //     GUILayout.EndHorizontal();
            //     if ( i != list.Count - 1 ) {
            //         DrawLine(1);
            //     }
            // }
            // GUILayout.Space(10);
            // DrawLine(3);
            // GUILayout.Space(10);
            // width = Screen.width / 3f;
            // GUILayout.BeginHorizontal();
            // GUILayout.Label("Star System", EditorStyles.boldLabel, GUILayout.Width(width));
            // GUILayout.Label("Faction", EditorStyles.boldLabel, GUILayout.Width(width));
            // GUILayout.Label("Start money", EditorStyles.boldLabel, GUILayout.Width(width));
            // GUILayout.EndHorizontal();
            // foreach ( var startInfo in graphInfo.GetStarSystemStartInfosInEditor() ) {
            //     GUILayout.BeginHorizontal();
            //     GUILayout.Label(startInfo.Name, GUILayout.Width(width));
            //     GUILayout.Label(startInfo.Faction.ToString(), GUILayout.Width(width));
            //     GUILayout.Label(startInfo.StartMoney.ToString(), GUILayout.Width(width));
            //     GUILayout.EndHorizontal();
            // }
            // GUILayout.Space(10);
            // DrawLine(3);
            // GUILayout.Space(10);
        }

        void DrawLine(float height) {
            var rect = EditorGUILayout.GetControlRect(false, height);
            EditorGUI.DrawRect(rect, SeparatorLineColor);
        }

        void OnSceneGUI(SceneView sceneView) {
            if ( StarSystemsGraphEditorWindow.Instance ) {
                DrawDistances(sceneView, StarSystemsGraphEditorWindow.Instance);
            }
        }
        
        void DrawDistances(SceneView sceneView, StarSystemsGraphEditorWindow window) {
            var starSystemsComps = FindObjectsOfType<BaseStarSystem>();
            // var dict = new Dictionary<string, BaseStarSystem>();
            // foreach ( var starSystemName in window.StarSystems ) {
            //     var success = false;
            //     foreach ( var comp in starSystemsComps ) {
            //         if ( comp.Name == starSystemName ) {
            //             dict.Add(starSystemName, comp);
            //             success = true;
            //             break;
            //         }
            //     }
            //     if ( !success ) {
            //         Debug.LogErrorFormat("Can't find BaseStarSystem component for graph star system '{0}'",
            //             starSystemName);
            //         return;
            //     }
            // }
            foreach ( var comp in starSystemsComps ) {
                foreach ( var otherComp in starSystemsComps ) {
                    if ( comp.Name == otherComp.Name ) {
                        continue;
                    }
                    var newPair      = (comp.Name, otherComp.Name);
                    var compPos      = comp.transform.position;
                    var otherCompPos = otherComp.transform.position;
                    var lineCenter   = (compPos + otherCompPos) / 2f;
                    var distance     = window.GetDistance(comp.Name, otherComp.Name);
                    var sceneSize    = sceneView.camera.orthographicSize;
                    var style        = new GUIStyle(GUI.skin.label) {
                        normal = {
                            textColor = Color.red,
                        },
                        fontSize      = Mathf.CeilToInt(3400 / sceneSize),
                        alignment     = TextAnchor.MiddleCenter,
                        stretchWidth  = true,
                        stretchHeight = true,
                        contentOffset = new Vector2(-350f / sceneSize, -1400f / sceneSize),
                    };
                    if ( Mathf.Approximately(distance, 0f) ) {
                        Handles.DrawDottedLine(compPos, otherCompPos, 7f);
                    } else {
                        Handles.DrawLine(compPos, otherCompPos);
                    }
                    var oldPrimaryColor = Handles.color;
                    Handles.color = (newPair == _selectedPair) ? Color.green : Color.white;
                    var buttonSize = 15f;
                    Handles.DrawSolidArc(lineCenter, new Vector3(0, 0, 1), Vector2.right, 360, buttonSize);
                    if ( Handles.Button(lineCenter,
                        Quaternion.identity, buttonSize, buttonSize, Handles.CircleHandleCap) ) {
                        _selectedPair = (_selectedPair == newPair) ? default : newPair; 
                        Repaint();
                    }
                    Handles.color = oldPrimaryColor;
                    Handles.Label(lineCenter, (distance <= 0) ? "+" : distance.ToString(), style);
                }
            }
        }

        void DrawSelectedPairInspector() {
            if ( !StarSystemsGraphEditorWindow.Instance || (_selectedPair == default) ) {
                return;
            }
            GUILayout.Label($"{_selectedPair.a} x {_selectedPair.b}");
            var window = StarSystemsGraphEditorWindow.Instance;
            var text = EditorGUILayout.TextField("Distance",
                window.GetDistance(_selectedPair.a, _selectedPair.b).ToString());
            if ( int.TryParse(text, out var distance) && (distance >= 0) ) {
                window.SetDistance(_selectedPair.a, _selectedPair.b, distance);
                window.Repaint();
            }
        }
    }
}
