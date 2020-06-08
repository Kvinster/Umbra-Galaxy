using UnityEngine;

using System.Collections.Generic;

using STP.Utils;
using STP.View;

namespace STP.Gameplay {
    public class EnemyShip : CoreBehaviour, IDestructable {
        const float CloseRadius = 10f;
        const float Speed       = 200f;
        
        public List<Transform> Route;
        public int             StartRoutePointIndex;
        
        int _hp = 2;
        int _nextRoutePoint;
        
        Rigidbody2D _rigidbody2D;
        
        MaterialCreator _materialCreator;
        
        Vector3 NextPoint       => Route[_nextRoutePoint].position;
        Vector2 MovingVector    => (NextPoint - transform.position);
        Vector2 MovingDirection => MovingVector.normalized;
        
        bool    CloseToPoint    => MovingVector.magnitude < CloseRadius;
        
        protected override void CheckDescription() { }
        

        public override void Init(CoreStarter starter) {
            _materialCreator   = starter.MaterialCreator;
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

        public void GetDamage(int damageAmount = 1) {
            _hp -= damageAmount;
            if ( _hp <= 0 ) {
                _materialCreator.CreateRandomMaterial(transform.position);
                Destroy(gameObject);
            } 
        }
    }
}