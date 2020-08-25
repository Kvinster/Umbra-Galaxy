using UnityEngine;

using STP.State.Core;
using STP.Utils;

namespace STP.Gameplay {
    public class FollowCamera : GameComponent {
        public Camera        Camera;
        public Transform     CameraTransform;
    
        CoreShipState _state;
    
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, Camera, CameraTransform);

        public void UpdatePos(Vector2 playerPosition) {
            CameraTransform.position = new Vector3(playerPosition.x, playerPosition.y, -10f);
        }
        
        void Start() {
            Camera.orthographicSize = Screen.height / 2.0f;
        }

    }
}
