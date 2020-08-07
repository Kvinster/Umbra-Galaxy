using UnityEngine;

using STP.Utils;

namespace STP.Behaviour.Meta {
    [RequireComponent(typeof(Camera))]
    public class CameraController : GameBehaviour {
        static readonly Vector2 TopRightBorder      = new Vector2(1920, 1080);
        static readonly Vector2 BottomLeftBorder    = new Vector2(-1920, -1080);
        
        [Range(0, 100)]
        public float MovementSpeed = 10f;

        Camera _camera;

        Vector2 _mousePos;

        void Start() {
            _camera   = GetComponent<Camera>();
            _mousePos = Input.mousePosition;
        }

        void Update() {
            var newMousePos = Input.mousePosition;
            if ( Input.GetMouseButton(1) && (Vector2.Distance(newMousePos, _mousePos) > 1f) ) {
                var delta = _camera.ScreenToWorldPoint(_mousePos) - _camera.ScreenToWorldPoint(newMousePos);
                transform.Translate(delta * MovementSpeed);
                ClampPosition();
            }
            _mousePos = newMousePos;
        }

        void ClampPosition() {
            var pos = transform.position;
            if ( pos.x < BottomLeftBorder.x ) {
                transform.Translate(Vector2.right * (BottomLeftBorder.x - pos.x));
            } else if ( pos.x > TopRightBorder.x ) {
                transform.Translate(Vector2.left * (pos.x - TopRightBorder.x));
            }
            if ( pos.y < BottomLeftBorder.y ) {
                transform.Translate(Vector2.up * (BottomLeftBorder.y - pos.y));
            } else if ( pos.y > TopRightBorder.y ) {
                transform.Translate(Vector2.down * (pos.y - TopRightBorder.y));
            }
        }
    }
}
