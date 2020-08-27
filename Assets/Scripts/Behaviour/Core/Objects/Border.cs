using UnityEditor;
using UnityEngine;

using STP.Behaviour.Starter;
using STP.Gameplay;
using STP.State.Core;

namespace STP.Behaviour.Core.Objects {
    public class Border : CoreComponent {
        public int BorderRadius = 2000;
        
        CoreShipState      _playerState;
        SelfDestructEngine _selfDestructEngine;
        
        public override void Init(CoreStarter starter) {
            _playerState        = starter.CoreManager.CorePlayerShipState;
            _selfDestructEngine = starter.CoreManager.SelfDestructEngine;
        }

        void Update() {
            var distance = (_playerState.Position - (Vector2)transform.position).magnitude;
            
            if ( distance > BorderRadius && !_selfDestructEngine.IsActive) {
                _selfDestructEngine.StartSelfDestruction();
            }
            if ( distance <= BorderRadius && _selfDestructEngine.IsActive) {
                _selfDestructEngine.StopSelfDestruction();
            }
        }

        void OnDrawGizmos() {
            Handles.DrawWireDisc(transform.position, Vector3.forward, BorderRadius);
        }
    }
}