using UnityEngine;

using System;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

using Shapes;

namespace STP.Behaviour.Core.Enemy {
    public class Connector : GameComponent {
        [NotNull]
        public Line Line;

        [HideInInspector]
        public Generator Other;

        bool _isInit;

        public bool IsInit {
            get => _isInit;
            private set {
                _isInit = value;
                if ( _isInit ) {
                    OnInit?.Invoke();
                }
            }
        }

        public event Action OnInit;

        public void Init(Generator one, Generator other) {
            Other = other;
            Line.Start = one.transform.position;
            Line.End   = other.transform.position;

            IsInit = true;
        }

        public void DestroyConnector() {
            Destroy(gameObject);
        }
    }
}