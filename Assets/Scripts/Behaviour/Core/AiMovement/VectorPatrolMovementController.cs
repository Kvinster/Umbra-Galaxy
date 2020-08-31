using UnityEngine;

using System.Collections.Generic;

using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.AiMovement {
    public sealed class VectorPatrolMovementController : BasePatrolMovementController {
        [NotNullOrEmpty(checkPrefab: false)]
        public List<Vector2> PatrolRoute = new List<Vector2>();
        
        protected override int PointsCount => PatrolRoute.Count;
        
        protected override bool CanDrawDizmo() => true;

        protected override Vector2 GetPoint(int index) {
            return PatrolRoute[index];
        }
    }
}
