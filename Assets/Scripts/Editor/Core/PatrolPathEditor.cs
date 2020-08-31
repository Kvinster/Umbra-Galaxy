using UnityEditor;
using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Core.AiMovement;

namespace STP.Editor.Core {
    public class PatrolPathEditor : EditorWindow {
        VectorPatrolMovementController _patrolMovementController;
        
        bool _cyclePath;
        
        bool _fixExistingPoint;
        int  _fixPointIndex;
        
        List<Vector2> Route => _patrolMovementController.PatrolRoute;
        
        [MenuItem("Core/Patrol Path Constructor")]
        static void Init() {
            var window = (PatrolPathEditor)GetWindow(typeof(PatrolPathEditor));
            window.Show();
        }

        void OnEnable() {
            SceneView.duringSceneGui += OnScene;
        }

        void OnDisable() {
            SceneView.duringSceneGui -= OnScene;
        }

        void OnScene(SceneView scene) {
            var e = Event.current;
            if ( !_patrolMovementController ) {
                return;
            }
            
            if ( IsLefButtomClick(e) ) {
                var mousePos = e.mousePosition;
                //reversing Y and fixing coords for different Windows 10 UI scale factor
                var ppp = EditorGUIUtility.pixelsPerPoint;
                mousePos.y = scene.camera.pixelHeight - mousePos.y * ppp;
                mousePos.x *= ppp;
                //Checking intersection
                if ( !HasIntersection(scene, mousePos) ) {
                    //Converting mouse position to the world position
                    var point = (Vector2)scene.camera.ScreenToWorldPoint(mousePos);
                    var pointIndex = (_fixExistingPoint) ? _fixPointIndex : Route.Count;
                    UpdatePath(point, pointIndex);
                    Event.current.Use();
                }
            }
            if ( IsRightButtonClick(e) ) {
                if ( Route.Count != 0 ) {
                    Route.RemoveAt(Route.Count - 1);
                    EditorUtility.SetDirty(_patrolMovementController);
                }
                Event.current.Use();
            }
        }

        bool IsLefButtomClick(Event e) {
            return IsButtonClick(e, 0);
        }

        bool IsRightButtonClick(Event e) {
            return IsButtonClick(e, 1);
        }

        bool IsButtonClick(Event e, int buttonIndex) {
            return (e.type == EventType.MouseDown) && (e.button == buttonIndex); 
        }

        bool HasIntersection(SceneView scene, Vector2 mousePos) {
            var ray = scene.camera.ScreenPointToRay(mousePos);
            var hits = Physics2D.RaycastAll(ray.origin, ray.direction);
            foreach ( var hit in hits ) {
                if ( !hit.collider.isTrigger ) {
                    Debug.LogError($"Overlapped with object { hit.collider.gameObject }");
                    return true;
                }
            }
            return false;
        }
        
        void UpdatePath(Vector2 newCoords, int routePointIndex) {
            if ( (routePointIndex < 0) || (routePointIndex > Route.Count ) ) {
                Debug.LogError($"Wrong index {routePointIndex}. Can't update point");
                return;
            }

            if ( routePointIndex == Route.Count ) {
                Route.Add(newCoords);
            }
            else {
                Route[routePointIndex] = newCoords;
            }
            EditorUtility.SetDirty(_patrolMovementController);
        }

        void OnGUI() {
            _patrolMovementController = EditorGUILayout.ObjectField("Current object", _patrolMovementController, typeof(VectorPatrolMovementController), true) as VectorPatrolMovementController;
            
            if ( _patrolMovementController ) {
                _patrolMovementController.IsCycledRoute = EditorGUILayout.Toggle("Cycle path", _patrolMovementController.IsCycledRoute);
            }
            
            _fixExistingPoint = EditorGUILayout.Toggle("Fix existing point", _fixExistingPoint);
            if ( _fixExistingPoint ) {
                _fixPointIndex = EditorGUILayout.IntField("Point index", _fixPointIndex);
            }
        }
    }
}