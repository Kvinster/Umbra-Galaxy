using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections.Generic;
using System.Linq;

using STP.Behaviour.Meta;
using STP.Common;

namespace STP.Editor.Meta {
    public class StarSystemsGraphEditorWindow : EditorWindow {
        public static StarSystemsGraphEditorWindow Instance { get; private set; }
        
        StarSystemsGraphInfo _graphInfo;
        
        public readonly List<string> StarSystems = new List<string>();
        public readonly HashSet<StarSystemsGraphInfo.StarSystemPair> Pairs =
            new HashSet<StarSystemsGraphInfo.StarSystemPair>();
        public readonly List<StarSystemsGraphInfo.StarSystemStartInfo> StartInfos =
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

        public int GetDistance(string aStarSystem, string bStarSystem) {
            if ( aStarSystem == bStarSystem ) {
                return 0;
            }
            return GetPair(aStarSystem, bStarSystem)?.Distance ?? 0;
        }
        
        public void SetDistance(string aStarSystem, string bStarSystem, int distance) {
            var pair = GetPair(aStarSystem, bStarSystem);
            if ( pair != null ) {
                pair.Distance = distance;
            }
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
                if ( !StarSystems.Contains(starSystem) ) {
                    StarSystems.Add(starSystem);
                }
            }

            void TryAddStartInfo(string starSystem) {
                if ( StartInfos.All(x => x.Name != starSystem) ) {
                    StartInfos.Add(new StarSystemsGraphInfo.StarSystemStartInfo { 
                        Name       = starSystem,
                        Faction    = Faction.Unknown,
                        StartMoney = 0
                    });
                }
            }
            
            StarSystems.Clear();
            Pairs.Clear();
            StartInfos.Clear();
            foreach ( var startInfo in _graphInfo.GetStarSystemStartInfosInEditor() ) {
                TryAddStarSystem(startInfo.Name);
                StartInfos.Add(new StarSystemsGraphInfo.StarSystemStartInfo {
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
                Pairs.Add(new StarSystemsGraphInfo.StarSystemPair {
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
            StarSystems.Clear();
            Pairs.Clear();
            StartInfos.Clear();
            foreach ( var rootGameObject in rootGameObjects ) {
                foreach ( var starSystem in rootGameObject.GetComponentsInChildren<BaseStarSystem>() ) {
                    if ( StarSystems.Contains(starSystem.Name) ) {
                        Debug.LogErrorFormat("Duplicate star system name '{0}'", starSystem.Name);
                        continue;
                    }
                    StarSystems.Add(starSystem.Name);
                }
            }
            StarSystems.Sort();
            foreach ( var starSystem in StarSystems ) {
                foreach ( var otherStarSystem in StarSystems ) {
                    if ( starSystem == otherStarSystem ) {
                        continue;
                    }
                    if ( GetPair(starSystem, otherStarSystem, true) == null ) {
                        Pairs.Add(new StarSystemsGraphInfo.StarSystemPair {
                            A        = starSystem,
                            B        = otherStarSystem,
                            Distance = 0
                        });
                    }
                }
                StartInfos.Add(new StarSystemsGraphInfo.StarSystemStartInfo {
                    Name     = starSystem,
                    Faction  = Faction.Unknown,
                    Portrait = null
                });
            }
        }

        void DrawDistanceTable() {
            if ( StarSystems.Count == 0 ) {
                RefreshSystemsFromGraph();
                if ( StarSystems.Count == 0 ) {
                    return;
                }
            }
            var width = Screen.width * 0.6f / StarSystems.Count;
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(width));
            foreach ( var starSystem in StarSystems ) {
                GUILayout.Label(starSystem, EditorStyles.boldLabel, GUILayout.Width(width));
            }
            GUILayout.EndHorizontal();
            foreach ( var starSystem in StarSystems ) {
                GUILayout.BeginHorizontal();
                for ( var i = 0; i < StarSystems.Count + 1; ++i ) {
                    if ( i == 0 ) {
                        GUILayout.Label(starSystem, EditorStyles.boldLabel, GUILayout.Width(width));
                    } else {
                        var otherStarSystem = StarSystems[i - 1];
                        if ( starSystem == otherStarSystem ) {
                            GUI.enabled = false;
                            GUILayout.TextField("0",GUILayout.Width(width));
                            GUI.enabled = true;
                        } else {
                            var text = GUILayout.TextField(GetDistance(starSystem, otherStarSystem).ToString(),
                                GUILayout.Width(width));
                            if ( int.TryParse(text, out var distance) && (distance >= 0) ) {
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
            foreach ( var startInfo in StartInfos ) {
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
            foreach ( var pair in Pairs ) {
                graphPairs.Add(pair);
            }
            var graphStartInfos = _graphInfo.GetStarSystemStartInfosInEditor();
            graphStartInfos.Clear();
            foreach ( var startInfo in StartInfos ) {
                graphStartInfos.Add(startInfo);
            }
            EditorUtility.SetDirty(_graphInfo);
        }

        StarSystemsGraphInfo.StarSystemPair GetPair(string aStarSystem, string bStarSystem, bool silent = false) {
            if ( aStarSystem == bStarSystem ) {
                return null;
            }
            foreach ( var pair in Pairs ) {
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

        void OnEnable() {
            Instance = this;
            if ( _graphInfo ) {
                RefreshSystemsFromGraph();
            } else {
                RefreshSystemsFromActiveScene();
            }
        }

        void OnDisable() {
            if ( Instance == this ) {
                Instance = null;
            }
        }

        [MenuItem("Meta/StarSystemsGraphEditor")]
        public static void Init() {
            var window = (StarSystemsGraphEditorWindow) GetWindow(typeof(StarSystemsGraphEditorWindow));
            window.titleContent = new GUIContent("Star Systems Graph Editor");
            window.Show();
        }
    }
}
