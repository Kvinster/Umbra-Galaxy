using UnityEngine;

using System.Collections.Generic;
using System.Linq;

using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.AiMovement {
    public sealed class TransformPatrolMovementController : BasePatrolMovementController {
        [NotNullOrEmpty(checkPrefab: false)]
        public List<Transform> PatrolRoute = new List<Transform>();

        protected override int PointsCount => PatrolRoute.Count;
        
        protected override bool CanDrawDizmo() {
            return PatrolRoute.Any(point => !point);
        }

        protected override Vector2 GetPoint(int index) {
            return PatrolRoute[index].position;
        }
    }
}
