using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

using STP.Behaviour.Starter;

namespace STP.Gameplay {
    public abstract class RoutedShip : BaseShip {
        const float CloseRadius     = 10f;

        public List<Transform> Route;
        public int             StartRoutePointIndex;

        public bool            CycledRoute;

        int   _nextRoutePoint;

        Vector3 NextPoint       => Route[_nextRoutePoint].position;
        Vector2 MovingVector    => (NextPoint - transform.position);
        Vector2 MovingDirection => MovingVector.normalized;
        bool    CloseToPoint    => MovingVector.magnitude < CloseRadius;

        public event Action<RoutedShip> ReachedRouteEnd;

        protected void Move() {
            if ( !CycledRoute && (_nextRoutePoint == 0) ) {
                Rigidbody2D.velocity = Vector2.zero;
                return;
            }
            Move(MovingDirection);
            Rotate(MovingDirection);
            if ( CloseToPoint ) {
                var nextPointIndex = (_nextRoutePoint + 1) % Route.Count;
                _nextRoutePoint = nextPointIndex;
                if ( !CycledRoute || (nextPointIndex == 0) ) {
                    ReachedRouteEnd?.Invoke(this);
                }
            }
        }

        public override void Init(CoreStarter starter) {
            transform.position = Route[StartRoutePointIndex].position;
            _nextRoutePoint    = (StartRoutePointIndex + 1) % Route.Count;
        }

        protected void OnDrawGizmos() {
            if ( Route.Any(point => !point) ) {
                return;
            }
            for ( var i = 0; i < Route.Count; i++ ) {
                var point  = Route[i];
                var point2 = Route[(i+1)%Route.Count];
                Gizmos.DrawWireSphere(point.position, 10f);
                if ( !CycledRoute && (i == (Route.Count - 1)) ) {
                    break;
                }
                Gizmos.DrawLine(point.position, point2.position);
            }
        }
    }
}