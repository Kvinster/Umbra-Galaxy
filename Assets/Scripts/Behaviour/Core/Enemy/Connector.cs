using UnityEngine;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

using Shapes;

namespace STP.Behaviour.Core.Enemy {
    public class Connector : GameComponent {
        [NotNull] 
        public Line Line;
        
        [HideInInspector] 
        public Generator Other;

        public void Init(Generator one, Generator other) {
            Other = other;
            Line.Start = one.transform.position;
            Line.End   = other.transform.position;
        }

        public void DestroyConnector() {
            Destroy(gameObject);
        }
    }
}