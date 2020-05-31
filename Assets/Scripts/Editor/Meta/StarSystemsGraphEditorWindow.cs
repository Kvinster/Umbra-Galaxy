using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections.Generic;

using STP.Behaviour.Meta;

namespace STP.Editor.Meta {
    public class StarSystemsGraphEditorWindow : EditorWindow {
        StarSystemsGraphInfo _graphInfo;
        
        readonly List<string> _starSystems = new List<string>();
        readonly HashSet<StarSystemsGraphInfo.StarSystemPair> _pairs =
            new HashSet<StarSystemsGraphInfo.StarSystemPair>();
        
        void OnGUI() {
            if ( _graphInfo ) {
                DrawWithGraph();
            } else {
                DrawNoGraph();
            }
        }

        public void SetGraph(StarSystemsGraphInfo graphInfo) {
            _graphInfo = graphInfo;
            RefreshSystemsFromGraph();
        }

        void DrawNoGraph() {
            GUILayout.Label("StarSystemsGraphInfo required", EditorStyles.boldLabel);
            _graphInfo = EditorGUILayout.ObjectField(new GUIContent("Star Systems Graph Info"), _graphInfo,
                typeof(StarSystemsGraphInfo), false) as StarSystemsGraphInfo;
            if ( _graphInfo ) {
                RefreshSystemsFromGraph();
            }
        }

        void RefreshSystemsFromGraph() {
            _starSystems.Clear();
            _pairs.Clear();
            foreach ( var pair in _graphInfo.GetStarSystemPairsInEditor() ) {
                if ( !_starSystems.Contains(pair.A) ) {
                    _starSystems.Add(pair.A);
                }
                if ( !_starSystems.Contains(pair.B) ) {
                    _starSystems.Add(pair.B);
                }
                _pairs.Add(new StarSystemsGraphInfo.StarSystemPair {
                    A        = pair.A,
                    B        = pair.B,
                    Distance = pair.Distance
                });
            }
        }

        void DrawWithGraph() {
            _graphInfo = EditorGUILayout.ObjectField(new GUIContent("Star Systems Graph Info"), _graphInfo,
                typeof(StarSystemsGraphInfo), false) as StarSystemsGraphInfo;
            if ( GUILayout.Button("Refresh systems") ) {
                RefreshSystems();
            }
            DrawTable();
            if ( GUILayout.Button("Save") ) {
                Save();
            }
        }

        void RefreshSystems() {
            var scene = SceneManager.GetActiveScene();
            if ( scene == default ) {
                return;
            }
            var rootGameObjects = scene.GetRootGameObjects();
            _starSystems.Clear();
            _pairs.Clear();
            foreach ( var rootGameObject in rootGameObjects ) {
                foreach ( var starSystem in rootGameObject.GetComponentsInChildren<BaseStarSystem>() ) {
                    if ( _starSystems.Contains(starSystem.Name) ) {
                        Debug.LogErrorFormat("Duplicate star system name '{0}'", starSystem.Name);
                        continue;
                    }
                    _starSystems.Add(starSystem.Name);
                }
            }
            _starSystems.Sort();
            foreach ( var starSystem in _starSystems ) {
                foreach ( var otherStarSystem in _starSystems ) {
                    if ( starSystem == otherStarSystem ) {
                        continue;
                    }
                    if ( GetPair(starSystem, otherStarSystem, true) == null ) {
                        _pairs.Add(new StarSystemsGraphInfo.StarSystemPair {
                            A        = starSystem,
                            B        = otherStarSystem,
                            Distance = 0
                        });
                    }
                }
            }
        }

        void DrawTable() {
            if ( _starSystems.Count == 0 ) {
                RefreshSystemsFromGraph();
                if ( _starSystems.Count == 0 ) {
                    return;
                }
            }
            var width = Screen.width * 0.6f / _starSystems.Count;
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(width));
            foreach ( var starSystem in _starSystems ) {
                GUILayout.Label(starSystem, EditorStyles.boldLabel, GUILayout.Width(width));
            }
            GUILayout.EndHorizontal();
            foreach ( var starSystem in _starSystems ) {
                GUILayout.BeginHorizontal();
                for ( var i = 0; i < _starSystems.Count + 1; ++i ) {
                    if ( i == 0 ) {
                        GUILayout.Label(starSystem, EditorStyles.boldLabel, GUILayout.Width(width));
                    } else {
                        var otherStarSystem = _starSystems[i - 1];
                        if ( starSystem == otherStarSystem ) {
                            GUI.enabled = false;
                            GUILayout.TextField("0",GUILayout.Width(width));
                            GUI.enabled = true;
                        } else {
                            var text = GUILayout.TextField(GetDistance(starSystem, otherStarSystem).ToString(),
                                GUILayout.Width(width));
                            if ( int.TryParse(text, out var distance) && (distance > 0) ) {
                                SetDistance(starSystem, otherStarSystem, distance);
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
        }

        void Save() {
            var graphPairs = _graphInfo.GetStarSystemPairsInEditor();
            graphPairs.Clear();
            foreach ( var pair in _pairs ) {
                graphPairs.Add(pair);
            }
            EditorUtility.SetDirty(_graphInfo);
        }

        int GetDistance(string aStarSystem, string bStarSystem) {
            if ( aStarSystem == bStarSystem ) {
                return 0;
            }
            return GetPair(aStarSystem, bStarSystem)?.Distance ?? 0;
        }
        
        void SetDistance(string aStarSystem, string bStarSystem, int distance) {
            var pair = GetPair(aStarSystem, bStarSystem);
            if ( pair != null ) {
                pair.Distance = distance;
            }
        }

        StarSystemsGraphInfo.StarSystemPair GetPair(string aStarSystem, string bStarSystem, bool silent = false) {
            if ( aStarSystem == bStarSystem ) {
                return null;
            }
            foreach ( var pair in _pairs ) {
                if ( ((pair.A == aStarSystem) && (pair.B == bStarSystem)) ||
                     ((pair.A == bStarSystem) && (pair.B == aStarSystem)) ) {
                    return pair;
                }
            }
            if ( !silent ) {
                Debug.LogErrorFormat("Can't find pair for ('{0}', '{1}')", aStarSystem, bStarSystem);
            }
            return null;
        }
        
        [MenuItem("Meta/StarSystemsGraphEditor")]
        public static void Init() {
            var window = (StarSystemsGraphEditorWindow) GetWindow(typeof(StarSystemsGraphEditorWindow));
            window.titleContent = new GUIContent("Star Systems Graph Editor");
            window.Show();
        }
    }
}
