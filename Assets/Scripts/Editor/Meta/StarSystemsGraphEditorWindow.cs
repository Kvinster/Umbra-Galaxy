using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections.Generic;

using STP.Behaviour.Meta;
using STP.Common;

namespace STP.Editor.Meta {
    public class StarSystemsGraphEditorWindow : EditorWindow {
        public static StarSystemsGraphEditorWindow Instance { get; private set; }

        Vector2 _scrollPos;

        StarSystemsGraphInfoScriptableObject _graphInfoScriptableObject;
        StarSystemsGraphInfo _graphInfo;

        void OnGUI() {
            if ( !_graphInfoScriptableObject ) {
                _graphInfo = null;
            }
            if ( _graphInfo != null ) {
                DrawWithGraph();
            } else {
                DrawNoGraph();
            }
        }

        public void SetGraph(StarSystemsGraphInfoScriptableObject graphInfoScriptableObject) {
            if ( graphInfoScriptableObject ) {
                _graphInfoScriptableObject = graphInfoScriptableObject;
                _graphInfo = _graphInfoScriptableObject.StarSystemsGraphInfo.Clone();
            } else {
                _graphInfoScriptableObject = null;
                _graphInfo = null;
            }
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
            _graphInfoScriptableObject = EditorGUILayout.ObjectField(new GUIContent("Star Systems Graph Info"),
                    _graphInfoScriptableObject, typeof(StarSystemsGraphInfoScriptableObject), false) as
                StarSystemsGraphInfoScriptableObject;
            if ( _graphInfoScriptableObject ) {
                _graphInfo = _graphInfoScriptableObject.StarSystemsGraphInfo.Clone();
            }
        }

        void DrawWithGraph() {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, false);
            _graphInfoScriptableObject = EditorGUILayout.ObjectField(new GUIContent("Star Systems Graph Info"),
                    _graphInfoScriptableObject, typeof(StarSystemsGraphInfoScriptableObject), false) as
                StarSystemsGraphInfoScriptableObject;
            if ( !_graphInfoScriptableObject ) {
                _graphInfo = null;
                return;
            }
            GUILayout.Space(10);
            DrawDistanceTable();
            GUILayout.Space(10);
            DrawStartInfos();
            GUILayout.Space(10);
            if ( GUILayout.Button("Refresh systems from graph") ) {
                _graphInfo = _graphInfoScriptableObject.StarSystemsGraphInfo.Clone();
            }
            if ( GUILayout.Button("Refresh systems from active scene") ) {
                RefreshSystemsFromActiveScene();
            }
            if ( GUILayout.Button("Save") ) {
                Save();
            }
            GUILayout.EndScrollView();
        }

        void RefreshSystemsFromActiveScene() {
            var scene = SceneManager.GetActiveScene();
            if ( scene == default ) {
                return;
            }
            var rootGameObjects = scene.GetRootGameObjects();

            var starSystems = new List<string>();
            var pairs       = _graphInfo.GetStarSystemPairsInEditor();
            var startInfos  = _graphInfo.GetStarSystemStartInfosInEditor();
            pairs.Clear();
            startInfos.Clear();
            foreach ( var rootGameObject in rootGameObjects ) {
                foreach ( var starSystem in rootGameObject.GetComponentsInChildren<BaseStarSystem>() ) {
                    if ( starSystems.Contains(starSystem.Name) ) {
                        Debug.LogErrorFormat("Duplicate star system name '{0}'", starSystem.Name);
                        continue;
                    }
                    starSystems.Add(starSystem.Name);
                }
            }
            starSystems.Sort();
            foreach ( var starSystem in starSystems ) {
                foreach ( var otherStarSystem in starSystems ) {
                    if ( starSystem == otherStarSystem ) {
                        continue;
                    }
                    if ( GetPair(starSystem, otherStarSystem, true) == null ) {
                        pairs.Add(new StarSystemPair {
                            A        = starSystem,
                            B        = otherStarSystem,
                            Distance = 0
                        });
                    }
                }
                startInfos.Add(new StarSystemsGraphInfo.StarSystemStartInfo {
                    Name     = starSystem,
                    Faction  = Faction.A,
                    Portrait = null
                });
            }
            if ( !_graphInfo.CheckValidity() ) {
                Debug.LogError("Invalid scene star systems setup");
                pairs.Clear();
                startInfos.Clear();
            }
        }

        void DrawDistanceTable() {
            if ( _graphInfo == null ) {
                return;
            }
            var starSystems = _graphInfo.StarSystems;
            if ( (starSystems == null) || (starSystems.Count == 0) ) {
                return;
            }
            var width = Screen.width * 0.6f / starSystems.Count;
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(width));
            foreach ( var starSystem in starSystems ) {
                GUILayout.Label(starSystem, EditorStyles.boldLabel, GUILayout.Width(width));
            }
            GUILayout.EndHorizontal();
            foreach ( var starSystem in starSystems ) {
                GUILayout.BeginHorizontal();
                for ( var i = 0; i < starSystems.Count + 1; ++i ) {
                    if ( i == 0 ) {
                        GUILayout.Label(starSystem, EditorStyles.boldLabel, GUILayout.Width(width));
                    } else {
                        var otherStarSystem = starSystems[i - 1];
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
            GUILayout.Label("Faction",     EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.2f));
            GUILayout.Label("Start Money", EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.2f));
            GUILayout.Label("Portrait",    EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.2f));
            GUILayout.EndHorizontal();
            var startInfos = _graphInfo.GetStarSystemStartInfosInEditor();
            foreach ( var startInfo in startInfos ) {
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
            _graphInfoScriptableObject.StarSystemsGraphInfo = _graphInfo.Clone();
            EditorUtility.SetDirty(_graphInfoScriptableObject);
        }

        StarSystemPair GetPair(string aStarSystem, string bStarSystem, bool silent = false) {
            if ( aStarSystem == bStarSystem ) {
                return null;
            }
            foreach ( var pair in _graphInfo.GetStarSystemPairsInEditor() ) {
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
