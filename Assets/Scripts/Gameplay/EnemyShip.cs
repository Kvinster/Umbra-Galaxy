using UnityEngine;

using System.Collections.Generic;

using STP.Utils;

namespace STP.Gameplay {
    public class EnemyShip : GameBehaviour {
        const float CloseRadius = 10f;
        const float Speed       = 200f;
        
        public List<Transform> Route;
        public int             StartRoutePointIndex;
        
        int _nextRoutePoint;
        
        Rigidbody2D _rigidbody2D;
        
        Vector3 NextPoint       => Route[_nextRoutePoint].position;
        Vector2 MovingVector    => (NextPoint - transform.position);
        Vector2 MovingDirection => MovingVector.normalized;
        
        bool    CloseToPoint    => MovingVector.magnitude < CloseRadius;
        
        protected override void CheckDescription() {
            if ( Route.Count == 0 ) {
                Debug.LogError("Route count is 0", this);
            }

            if ( StartRoutePointIndex >= Route.Count ) {
                Debug.LogError(string.Format("Don't have route point with index {0}", StartRoutePointIndex), this);
            }
        }
        
        void Start() {
            _rigidbody2D       = GetComponent<Rigidbody2D>();
            transform.position = Route[StartRoutePointIndex].position;
            _nextRoutePoint    = (StartRoutePointIndex + 1) % Route.Count;
        }

        void Update() {
            var offsetVector = Speed * MovingDirection;
            MoveUtils.ApplyMovingVector(_rigidbody2D, transform, offsetVector);
            if ( CloseToPoint ) {
                _nextRoutePoint  = (_nextRoutePoint + 1) % Route.Count;
            }
        }

    }
}