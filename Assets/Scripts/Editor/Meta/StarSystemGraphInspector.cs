using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using System.Linq;

using STP.Behaviour.Meta;
using STP.Common;

namespace STP.Editor.Meta {
    [CustomEditor(typeof(StarSystemsGraphInfoScriptableObject))]
    public class StarSystemGraphInspector : UnityEditor.Editor {
        static readonly Color SeparatorLineColor = new Color(0.5f,0.5f,0.5f, 1);

        StarSystemsGraphEditorWindow _window;
        SceneView                    _sceneView;
        List<BaseStarSystem>         _allSystems     = null;
        List<BaseStarSystem>         _visibleSystems = null;

        (string a, string b) _selectedPair => ((_visibleSystems != null) && (_visibleSystems.Count == 2))
            ? (_visibleSystems[0].Name, _visibleSystems[1].Name)
            : default;

        string _selectedSystem => ((_visibleSystems != null) && (_visibleSystems.Count == 1))
            ? _visibleSystems[0].Name
            : string.Empty;

        void OnEnable() {
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        void OnDisable() {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }

        public override void OnInspectorGUI() {
            var graphInfo = target as StarSystemsGraphInfoScriptableObject;
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
                if ( GUILayout.Button("Select all") ) {
                    _visibleSystems.Clear();
                    _visibleSystems.AddRange(_allSystems);
                    _sceneView.Repaint();
                }
                if ( GUILayout.Button("Clear all") ) {
                    _visibleSystems.Clear();
                    _sceneView.Repaint();
                }
                DrawSelectedPairInspector();
                DrawSelectedSystemInspector();
            }
        }

        void DrawLine(float height) {
            var rect = EditorGUILayout.GetControlRect(false, height);
            EditorGUI.DrawRect(rect, SeparatorLineColor);
        }

        void OnSceneGUI(SceneView sceneView) {
            if ( StarSystemsGraphEditorWindow.Instance ) {
                _sceneView = sceneView;
                _window    = StarSystemsGraphEditorWindow.Instance;
                if ( _allSystems == null ) {
                    _allSystems = new List<BaseStarSystem>();
                    _allSystems.AddRange(FindObjectsOfType<BaseStarSystem>());
                }
                if ( _visibleSystems == null ) {
                    _visibleSystems = new List<BaseStarSystem>();
                    _visibleSystems.AddRange(_allSystems);
                }
                DrawDistances();
                DrawSystemButtons();
            }
        }

        void DrawSystemButtons() {
            var oldColor = Handles.color;
            var buttonSize = 20f;
            foreach ( var system in _allSystems ) {
                var isVisible = _visibleSystems.Contains(system);
                Handles.color = isVisible ? Color.red : Color.green;
                Handles.DrawSolidArc(system.transform.position, new Vector3(0, 0, 1), Vector2.right, 360, buttonSize);
                if ( Handles.Button(system.transform.position, Quaternion.identity, buttonSize, buttonSize,
                    Handles.CircleHandleCap) ) {
                    if ( isVisible ) {
                        _visibleSystems.Remove(system);
                    } else {
                        _visibleSystems.Add(system);
                    }
                    Repaint();
                }
            }
            Handles.color = oldColor;
        }
        
        void DrawDistances() {
            var starSystemsComps = FindObjectsOfType<BaseStarSystem>();
            if ( _selectedPair == default ) {
                foreach ( var comp in starSystemsComps ) {
                    if ( !_visibleSystems.Contains(comp) ) {
                        continue;
                    }
                    foreach ( var otherComp in starSystemsComps ) {
                        if ( !_visibleSystems.Contains(otherComp) ) {
                            continue;
                        }
                        DrawDistances(comp, otherComp);
                    }
                }
            } else {
                var aStarSystem = starSystemsComps.Where(x => x.Name == _selectedPair.a).ToList();
                var bStarSystem = starSystemsComps.Where(x => x.Name == _selectedPair.b).ToList();
                DrawDistances(aStarSystem[0], bStarSystem[0]);
            }
        }

        void DrawDistances(BaseStarSystem aStarSystem, BaseStarSystem bStarSystem) {
            if ( aStarSystem.Name == bStarSystem.Name ) {
                return;
            }
            var distance     = _window.GetDistance(aStarSystem.Name, bStarSystem.Name);
            if ( Mathf.Approximately(distance, 0f) ) {
                return;
            }
            var newPair      = (aStarSystem.Name, bStarSystem.Name);
            var compPos      = aStarSystem.transform.position;
            var otherCompPos = bStarSystem.transform.position;
            var lineCenter   = (compPos + otherCompPos) / 2f;
            var sceneSize    = _sceneView.camera.orthographicSize;
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
            var oldPrimaryColor = Handles.color;
            Handles.color = (newPair == _selectedPair) ? Color.green : Color.white;
            if ( Mathf.Approximately(distance, 0f) ) {
                Handles.DrawDottedLine(compPos, otherCompPos, 7f);
            } else {
                Handles.DrawLine(compPos, otherCompPos);
            }
            var buttonSize = 15f;
            Handles.DrawSolidArc(lineCenter, new Vector3(0, 0, 1), Vector2.right, 360, buttonSize);
            Handles.color = oldPrimaryColor;
            Handles.Label(lineCenter, distance.ToString(), style);
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

        void DrawSelectedSystemInspector() {
            if ( !StarSystemsGraphEditorWindow.Instance || string.IsNullOrEmpty(_selectedSystem) ) {
                return;
            }
            var window = StarSystemsGraphEditorWindow.Instance;
            
            GUILayout.Label(_selectedSystem);

            EditorGUI.BeginChangeCheck();
            
            var curFaction = window.GetFaction(_selectedSystem);
            curFaction = (Faction)EditorGUILayout.EnumPopup("Faction", curFaction);

            var curMoney = window.GetMoney(_selectedSystem);
            var newMoneyText = EditorGUILayout.TextField("Start Money", curMoney.ToString());
            if ( int.TryParse(newMoneyText, out var newMoney) && (newMoney >= 0) ) {
                curMoney = newMoney;
            }

            var curSurvivalChance     = window.GetSurvivalChance(_selectedSystem);
            var newSurvivalChanceText = EditorGUILayout.TextField("Survival Chance", curSurvivalChance.ToString());
            if ( int.TryParse(newSurvivalChanceText, out var newSurvivalChance) ) {
                curSurvivalChance = newSurvivalChance;
            }

            var curPortrait = window.GetPortrait(_selectedSystem); 
            curPortrait = EditorGUILayout.ObjectField("Portrait", curPortrait, typeof(Sprite), false) as Sprite;

            if ( EditorGUI.EndChangeCheck() ) {
                window.SetFaction(_selectedSystem, curFaction);
                window.SetMoney(_selectedSystem, curMoney);
                window.SetSurvivalChance(_selectedSystem, curSurvivalChance);
                window.SetPortrait(_selectedSystem, curPortrait);
                window.Repaint();
            }
        }
    }
}
