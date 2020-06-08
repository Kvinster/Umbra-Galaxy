using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections.Generic;

using STP.Behaviour.Meta;
using STP.Common;
using System.Linq;

namespace STP.Editor.Meta {
    public class StarSystemsGraphEditorWindow : EditorWindow {
        StarSystemsGraphInfo _graphInfo;
        
        readonly List<string> _starSystems = new List<string>();
        readonly HashSet<StarSystemsGraphInfo.StarSystemPair> _pairs =
            new HashSet<StarSystemsGraphInfo.StarSystemPair>();
        readonly List<StarSystemsGraphInfo.StarSystemStartInfo> _startInfos =
            new List<StarSystemsGraphInfo.StarSystemStartInfo>();
        
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

        void DrawWithGraph() {
            GUILayout.BeginScrollView(Vector2.zero, false, false);
            _graphInfo = EditorGUILayout.ObjectField(new GUIContent("Star Systems Graph Info"), _graphInfo,
                typeof(StarSystemsGraphInfo), false) as StarSystemsGraphInfo;
            GUILayout.Space(10);
            DrawDistanceTable();
            GUILayout.Space(10);
            DrawStartInfos();
            GUILayout.Space(10);
            if ( GUILayout.Button("Refresh systems from graph") ) {
                RefreshSystemsFromGraph();
            }
            if ( GUILayout.Button("Refresh systems from active scene") ) {
                RefreshSystemsFromActiveScene();
            }
            if ( GUILayout.Button("Save") ) {
                Save();
            }
            GUILayout.EndScrollView();
        }

        void RefreshSystemsFromGraph() {
            void TryAddStarSystem(string starSystem) {
                if ( !_starSystems.Contains(starSystem) ) {
                    _starSystems.Add(starSystem);
                }
            }

            void TryAddStartInfo(string starSystem) {
                if ( _startInfos.All(x => x.Name != starSystem) ) {
                    _startInfos.Add(new StarSystemsGraphInfo.StarSystemStartInfo { 
                        Name       = starSystem,
                        Faction    = Faction.Unknown,
                        StartMoney = 0
                    });
                }
            }
            
            _starSystems.Clear();
            _pairs.Clear();
            _startInfos.Clear();
            foreach ( var startInfo in _graphInfo.GetStarSystemStartInfosInEditor() ) {
                TryAddStarSystem(startInfo.Name);
                _startInfos.Add(new StarSystemsGraphInfo.StarSystemStartInfo {
                    Name       = startInfo.Name,
                    Faction    = startInfo.Faction,
                    StartMoney = startInfo.StartMoney,
                    Portrait   = startInfo.Portrait,
                });
            }
            foreach ( var pair in _graphInfo.GetStarSystemPairsInEditor() ) {
                TryAddStarSystem(pair.A);
                TryAddStarSystem(pair.B);
                TryAddStartInfo(pair.A);
                TryAddStartInfo(pair.B);
                _pairs.Add(new StarSystemsGraphInfo.StarSystemPair {
                    A        = pair.A,
                    B        = pair.B,
                    Distance = pair.Distance
                });
            }
        }

        void RefreshSystemsFromActiveScene() {
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

        void DrawDistanceTable() {
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

        void DrawStartInfos() {
            var headerStyle = new GUIStyle(GUI.skin.GetStyle("Label")) {
                alignment = TextAnchor.UpperCenter,
                fontStyle = FontStyle.Bold,
                fontSize  = 16
            };
            GUILayout.Label("Star Systems Common Info", headerStyle);
            var totalWidth = Screen.width - 20f;
            GUILayout.BeginHorizontal();
            GUILayout.Label("System Name", EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.15f));
            GUILayout.Label("Faction", EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.2f));
            GUILayout.Label("Start Money", EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.2f));
            GUILayout.Label("Portrait", EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.2f));
            GUILayout.EndHorizontal();
            foreach ( var startInfo in _startInfos ) {
                GUILayout.BeginHorizontal();
                GUILayout.Label(startInfo.Name, GUILayout.Width(totalWidth * 0.15f));
                startInfo.Faction =
                    (Faction) EditorGUILayout.EnumPopup(startInfo.Faction, GUILayout.Width(totalWidth * 0.2f));
                var startMoneyText =
                    GUILayout.TextField(startInfo.StartMoney.ToString(), GUILayout.Width(totalWidth * 0.2f));
                if ( int.TryParse(startMoneyText, out var startMoney) && (startMoney >= 0) ) {
                    startInfo.StartMoney = startMoney;
                }
                startInfo.Portrait = EditorGUILayout.ObjectField(startInfo.Portrait, typeof(Sprite), false) as Sprite;
                GUILayout.EndHorizontal();
            }
        }

        void Save() {
            var graphPairs = _graphInfo.GetStarSystemPairsInEditor();
            graphPairs.Clear();
            foreach ( var pair in _pairs ) {
                graphPairs.Add(pair);
            }
            var graphStartInfos = _graphInfo.GetStarSystemStartInfosInEditor();
            graphStartInfos.Clear();
            foreach ( var startInfo in _startInfos ) {
                graphStartInfos.Add(startInfo);
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
