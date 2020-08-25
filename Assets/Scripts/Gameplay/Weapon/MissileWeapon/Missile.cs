﻿using UnityEngine;

using System.Collections.Generic;
using System.Linq;

using STP.Behaviour.Core.Objects;
using STP.Gameplay.Weapon.GunWeapon;
using STP.Utils;

namespace STP.Gameplay.Weapon.MissileWeapon {
    public class Missile : Bullet {
        const float DetectionRadius  = 1000f;
        const float DetectoinDelay   = 0.3f;
        const float Velocity         = 400f;
        const int   DefaultLayerMask = 1 << 0;
        const int   PlayerLayerMask  = 1 << 8;
        
        int _layerMask = DefaultLayerMask;
        
        Timer _timer = new Timer();
        
        Transform   _target;
        
        Rigidbody2D _rigidbody;

        HashSet<ConflictSide> _availableTargets;
        
        public void Init(GameObject sourceShip, AllianceManager allianceManager) {
            base.Init(sourceShip);
            _timer.Start(DetectoinDelay);
            _rigidbody         = GetComponent<Rigidbody2D>();
            var sideComp       = sourceShip.GetComponent<ISideAccessable>();
            _layerMask         = ( sideComp.CurrentSide != ConflictSide.Player ) ? PlayerLayerMask : DefaultLayerMask;
            _availableTargets  = allianceManager.GetEnemiesSides(sideComp.CurrentSide);
            transform.rotation = sourceShip.transform.rotation * Quaternion.AngleAxis(180, Vector3.forward);
        }
        
        void Update() {
            if ( _timer.Tick(Time.deltaTime) ) {
                TryFindTarget();
                _timer.Stop();
            }
            TryMoveToTarget();
        }

        void TryFindTarget() {
            var hits = Physics2D.OverlapCircleAll(transform.position, DetectionRadius, _layerMask).ToList();
            hits.Sort((x, y) => HitsComparer(x, y));
            foreach ( var collider in hits ) {
                var comp = collider.gameObject.GetComponent<IDestructable>();
                var ship = collider.gameObject.GetComponent<ISideAccessable>();
                if ( (comp != null) && _availableTargets.Contains(ship?.CurrentSide ?? ConflictSide.Unknown) ) {
                    _target = collider.transform;
                    break;
                } 
            }
        }

        int HitsComparer(Collider2D x, Collider2D y) {
            var comp1 = x.GetComponent<BaseShip>();
            var comp2 = y.GetComponent<BaseShip>();
            if ( !comp1 && comp2) {
                return 1;
            }
            if ( !comp2 && comp1 ) {
                return -1;
            }
            return Vector2.Distance(x.transform.position, transform.position).CompareTo(Vector2.Distance(y.transform.position, transform.position));
        }

        void TryMoveToTarget() {
            if ( !_target ) {
                return;
            }
            var direction = (_target.position - transform.position).normalized;
            _rigidbody.velocity = Velocity * direction;
            MoveUtils.ApplyViewVector(transform, -direction);
        }

        void OnDrawGizmos() {
            Gizmos.DrawWireSphere(transform.position, DetectionRadius);
        }
    }
}