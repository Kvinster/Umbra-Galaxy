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

        public List<string> StarSystems => _graphInfo.StarSystems;

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

        public Faction GetFaction(string starSystem) {
            return (GetStartInfo(starSystem)?.Faction ?? Faction.Unknown);
        }

        public void SetFaction(string starSystem, Faction faction) {
            var startInfo = GetStartInfo(starSystem);
            if ( startInfo != null ) {
                startInfo.Faction = faction;
            }
        }

        public int GetMoney(string starSystem) {
            return (GetStartInfo(starSystem)?.StartMoney ?? -1);
        }

        public void SetMoney(string starSystem, int money) {
            var startInfo = GetStartInfo(starSystem);
            if ( startInfo == null ) {
                return;
            }
            if ( money < 0 ) {
                Debug.LogError("New start money for system '{0}' is less than zero");
                return;
            }
            startInfo.StartMoney = money;
        }

        public int GetSurvivalChance(string starSystem) {
            return (GetStartInfo(starSystem)?.BaseSurvivalChance ?? -1);
        }

        public void SetSurvivalChance(string starSystem, int survivalChance) {
            var startInfo = GetStartInfo(starSystem);
            if ( startInfo == null ) {
                return;
            }
            startInfo.BaseSurvivalChance = survivalChance;
        }

        public Sprite GetPortrait(string starSystem) {
            return GetStartInfo(starSystem)?.Portrait;
        }

        public void SetPortrait(string starSystem, Sprite portrait) {
            var startInfo = GetStartInfo(starSystem);
            if ( startInfo != null ) {
                startInfo.Portrait = portrait;
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
                _graphInfo.GetStarSystemStartInfosInEditor().Sort((a, b) => {
                    if ( a.Faction != b.Faction ) {
                        return (((int)a.Faction < (int) b.Faction) ? -1 : 1);
                    }
                    return string.CompareOrdinal(a.Name, b.Name);
                });
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
                startInfos.Add(new StarSystemStartInfo {
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
            // ReSharper disable once UseNullPropagation
            if ( _graphInfo == null ) {
                return;
            }
            var starSystems = _graphInfo.StarSystems;
            if ( (starSystems == null) || (starSystems.Count == 0) ) {
                return;
            }
            var width = Screen.width * 0.9f / (starSystems.Count + 1);
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
            var startInfos = _graphInfo.GetStarSystemStartInfosInEditor();
            if ( startInfos.Count == 0 ) {
                return;
            }
            var headerStyle = new GUIStyle(GUI.skin.GetStyle("Label")) {
                alignment = TextAnchor.UpperCenter,
                fontStyle = FontStyle.Bold,
                fontSize  = 16
            };
            GUILayout.Label("Star Systems Common Info", headerStyle);
            var totalWidth = Screen.width - 20f;
            GUILayout.BeginHorizontal();
            GUILayout.Label("System Name",     EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.15f));
            GUILayout.Label("Faction",         EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.2f));
            GUILayout.Label("Start Money",     EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.2f));
            GUILayout.Label("Survival Chance", EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.2f));
            GUILayout.Label("Portrait",        EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.2f));
            GUILayout.EndHorizontal();
            var prevFaction = startInfos[0].Faction;
            foreach ( var startInfo in startInfos ) {
                if ( startInfo.Faction != prevFaction ) {
                    GUILayout.Space(10);
                    prevFaction = startInfo.Faction;
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label(startInfo.Name, GUILayout.Width(totalWidth * 0.15f));
                startInfo.Faction =
                    (Faction) EditorGUILayout.EnumPopup(startInfo.Faction, GUILayout.Width(totalWidth * 0.2f));
                var startMoneyText =
                    GUILayout.TextField(startInfo.StartMoney.ToString(), GUILayout.Width(totalWidth * 0.2f));
                if ( int.TryParse(startMoneyText, out var startMoney) && (startMoney >= 0) ) {
                    startInfo.StartMoney = startMoney;
                }
                var survivalChanceText =
                    GUILayout.TextField(startInfo.BaseSurvivalChance.ToString(), GUILayout.Width(totalWidth * 0.2f));
                if ( int.TryParse(survivalChanceText, out var survivalChance) ) {
                    startInfo.BaseSurvivalChance = survivalChance;
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

        StarSystemStartInfo GetStartInfo(string starSystem, bool silent = false) {
            if ( string.IsNullOrEmpty(starSystem) ) {
                return null;
            }
            foreach ( var startInfo in _graphInfo.GetStarSystemStartInfosInEditor() ) {
                if ( startInfo.Name == starSystem ) {
                    return startInfo;
                }
            }
            if ( !silent ) {
                Debug.LogErrorFormat("Can't find start info for '{0}'", starSystem);
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
