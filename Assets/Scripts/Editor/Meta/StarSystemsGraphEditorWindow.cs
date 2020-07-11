using UnityEditor;
using UnityEngine;

using STP.Behaviour.Meta;
using STP.Common;

namespace STP.Editor.Meta {
    public class StarSystemsGraphEditorWindow : EditorWindow {
        public static StarSystemsGraphEditorWindow Instance { get; private set; }

        public StarSystemsGraphInfo GraphInfo;
        
        StarSystemsGraphInfoScriptableObject _graphInfoScriptableObject;

        Vector2 _scrollPos;

        GUIStyle DisabledLabelStyle => new GUIStyle(GUI.skin.label) {
            normal = new GUIStyleState() {
                textColor = Color.gray
            }
        };

        GUIStyle LabelButtonStyle => new GUIStyle(GUI.skin.label) {
            border = new RectOffset(0, 0, 0, 0)
        };

        GUIStyle HeaderStyle =>  new GUIStyle(GUI.skin.GetStyle("Label")) {
            alignment = TextAnchor.UpperCenter,
            fontStyle = FontStyle.Bold,
            fontSize  = 16
        };

        void OnGUI() {
            if ( !_graphInfoScriptableObject ) {
                GraphInfo = null;
            }
            if ( GraphInfo != null ) {
                DrawWithGraph();
            } else {
                DrawNoGraph();
            }
        }

        public void SetGraph(StarSystemsGraphInfoScriptableObject graphInfoScriptableObject) {
            if ( graphInfoScriptableObject ) {
                _graphInfoScriptableObject = graphInfoScriptableObject;
                GraphInfo = _graphInfoScriptableObject.StarSystemsGraphInfo.Clone();
            } else {
                _graphInfoScriptableObject = null;
                GraphInfo = null;
            }
        }

        void DrawNoGraph() {
            GUILayout.Label("StarSystemsGraphInfo required", EditorStyles.boldLabel);
            _graphInfoScriptableObject = EditorGUILayout.ObjectField(new GUIContent("Star Systems Graph Info"),
                    _graphInfoScriptableObject, typeof(StarSystemsGraphInfoScriptableObject), false) as
                StarSystemsGraphInfoScriptableObject;
            if ( _graphInfoScriptableObject ) {
                GraphInfo = _graphInfoScriptableObject.StarSystemsGraphInfo.Clone();
            }
        }

        void DrawWithGraph() {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, false);
            _graphInfoScriptableObject = EditorGUILayout.ObjectField(new GUIContent("Star Systems Graph Info"),
                    _graphInfoScriptableObject, typeof(StarSystemsGraphInfoScriptableObject), false) as
                StarSystemsGraphInfoScriptableObject;
            if ( !_graphInfoScriptableObject ) {
                GraphInfo = null;
                return;
            }
            GUILayout.Space(10);
            DrawFactionSystemInfos();
            DrawLine(3);
            DrawShardSystemsInfo();
            DrawLine(3);
            if ( GUILayout.Button("Refresh systems from graph") ) {
                GraphInfo = _graphInfoScriptableObject.StarSystemsGraphInfo.Clone();
                SortFactionStarSystemInfos();
            }
            if ( GUILayout.Button("Save") ) {
                Save();
            }
            GUILayout.EndScrollView();
        }

        void SortFactionStarSystemInfos() {
            GraphInfo.FactionSystemInfos.Sort((a, b) => {
                if ( a.Faction != b.Faction ) {
                    return (((int)a.Faction < (int) b.Faction) ? -1 : 1);
                }
                return string.CompareOrdinal(a.Name, b.Name);
            });
        }

        string _renamedStarSystemId;
        void DrawFactionSystemInfos() {
            var startInfos = GraphInfo.FactionSystemInfos;
            if ( startInfos.Count == 0 ) {
                return;
            }
            GUILayout.Label("Faction Star Systems Info", HeaderStyle);
            var totalWidth = Screen.width - 20f;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Id",              EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.05f));
            GUILayout.Label("System Name",     EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.1f));
            GUILayout.Label("Faction",         EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.1f));
            GUILayout.Label("Start Money",     EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.1f));
            GUILayout.Label("Survival Chance", EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.1f));
            GUILayout.Label("Portrait",        EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.2f));
            GUILayout.EndHorizontal();
            if ( Event.current.Equals (Event.KeyboardEvent ("return")) ) {
                _renamedStarSystemId = string.Empty;
            }
            var prevFaction = startInfos[0].Faction;
            foreach ( var startInfo in startInfos ) {
                if ( startInfo.Faction != prevFaction ) {
                    GUILayout.Space(10);
                    prevFaction = startInfo.Faction;
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label(startInfo.Id, DisabledLabelStyle, GUILayout.Width(totalWidth * 0.05f));
                var controlName = $"name_text_field_{startInfo.Id}";
                if ( _renamedStarSystemId == startInfo.Id ) {
                    GUI.SetNextControlName(controlName);
                    startInfo.Name = GUILayout.TextField(startInfo.Name, GUILayout.Width(totalWidth * 0.1f));
                    if ( GUI.GetNameOfFocusedControl() != controlName ) {
                        _renamedStarSystemId = string.Empty;
                    }
                } else {
                    if ( GUILayout.Button(startInfo.Name, LabelButtonStyle, GUILayout.Width(totalWidth * 0.1f)) ) {
                        _renamedStarSystemId = startInfo.Id;
                        GUI.FocusControl(controlName);
                    }
                }
                EditorGUI.BeginChangeCheck();
                startInfo.Faction =
                    (Faction) EditorGUILayout.EnumPopup(startInfo.Faction, GUILayout.Width(totalWidth * 0.1f));
                if ( EditorGUI.EndChangeCheck() ) {
                    SortFactionStarSystemInfos();
                    break;
                }
                var startMoneyText =
                    GUILayout.TextField(startInfo.StartMoney.ToString(), GUILayout.Width(totalWidth * 0.1f));
                if ( int.TryParse(startMoneyText, out var startMoney) && (startMoney >= 0) ) {
                    startInfo.StartMoney = startMoney;
                }
                var survivalChanceText =
                    GUILayout.TextField(startInfo.BaseSurvivalChance.ToString(), GUILayout.Width(totalWidth * 0.1f));
                if ( int.TryParse(survivalChanceText, out var survivalChance) ) {
                    startInfo.BaseSurvivalChance = survivalChance;
                }
                startInfo.Portrait = EditorGUILayout.ObjectField(startInfo.Portrait, typeof(Sprite), false) as Sprite;
                if ( GUILayout.Button("-", GUILayout.ExpandWidth(false)) ) {
                    GraphInfo.RemoveFactionStarSystem(startInfo.Id);
                    break;
                }
                GUILayout.EndHorizontal();
            }
            if ( GUILayout.Button("Add Faction Star System") ) {
                GraphInfo.AddFactionStarSystem();
            }
        }

        void DrawShardSystemsInfo() {
            GUILayout.Label("Shard Systems Info", HeaderStyle);
            var totalWidth = Screen.width - 20f;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Id",          EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.05f));
            GUILayout.Label("System Name", EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.1f));
            GUILayout.Label("Level",       EditorStyles.boldLabel, GUILayout.Width(totalWidth * 0.1f));
            GUILayout.EndHorizontal();
            foreach ( var shardInfo in GraphInfo.ShardSystemInfos ) {
                GUILayout.BeginHorizontal();
                GUILayout.Label(shardInfo.Id, DisabledLabelStyle, GUILayout.Width(totalWidth * 0.05f));
                var controlName = $"name_text_field_{shardInfo.Id}";
                if ( _renamedStarSystemId == shardInfo.Id ) {
                    GUI.SetNextControlName(controlName);
                    shardInfo.Name = GUILayout.TextField(shardInfo.Name, GUILayout.Width(totalWidth * 0.1f));
                    if ( GUI.GetNameOfFocusedControl() != controlName ) {
                        _renamedStarSystemId = string.Empty;
                    }
                } else {
                    if ( GUILayout.Button(shardInfo.Name, LabelButtonStyle, GUILayout.Width(totalWidth * 0.1f)) ) {
                        _renamedStarSystemId = shardInfo.Id;
                        GUI.FocusControl(controlName);
                    }
                }
                EditorGUI.BeginChangeCheck();
                var levelText =
                    GUILayout.TextField(shardInfo.Level.ToString(), GUILayout.Width(totalWidth * 0.1f));
                if ( int.TryParse(levelText, out var level) ) {
                    shardInfo.Level = level;
                }
                if ( GUILayout.Button("-", GUILayout.ExpandWidth(false)) ) {
                    GraphInfo.RemoveShardStarSystem(shardInfo.Id);
                    break;
                }
                GUILayout.EndHorizontal();
            }
            if ( GUILayout.Button("Add Shard Star System") ) {
                GraphInfo.AddShardStarSystem();
            }
        }

        void DrawLine(float height) {
            DrawLine(height, Color.black);
        }

        void DrawLine(float height, Color color) {
            GUILayout.Space(height);
            var rect = EditorGUILayout.GetControlRect(false, height);
            EditorGUI.DrawRect(rect, color);
            GUILayout.Space(height);
        }

        void Save() {
            _graphInfoScriptableObject.StarSystemsGraphInfo = GraphInfo.Clone();
            EditorUtility.SetDirty(_graphInfoScriptableObject);
        }

        StarSystemPair GetPair(string aStarSystemId, string bStarSystemId, bool silent = false) {
            if ( aStarSystemId == bStarSystemId ) {
                return null;
            }
            foreach ( var pair in GraphInfo.StarSystemPairs ) {
                if ( ((pair.A == aStarSystemId) && (pair.B == bStarSystemId)) ||
                     ((pair.A == bStarSystemId) && (pair.B == aStarSystemId)) ) {
                    return pair;
                }
            }
            if ( !silent ) {
                Debug.LogErrorFormat("Can't find pair for ('{0}', '{1}')", aStarSystemId, bStarSystemId);
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
