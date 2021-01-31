using System;
using System.Collections.Generic;

using STP.Utils;

using Shapes;

namespace STP.Behaviour.Core.Enemy {
    public sealed class Connector : GameComponent {
        public Connector       Parent;
        public List<Connector> Children;

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
        public event Action OnOutOfLinks;

        void Start() {
            foreach ( var line in GetComponentsInChildren<Line>() ) {
                if ( line.gameObject.name.StartsWith("Minimap") ) {
                    continue;
                }
                line.gameObject.AddComponent<ConnectorLine>();
            }
        }

        public void Init() {
            IsInit = true;
        }

        public void ForceDestroy() {
            try {
                foreach ( var link in Children ) {
                    link.ForceDestroy();
                }
            } catch ( Exception e ) {
                print("fuck");
            }
            Destroy(gameObject);
            OnOutOfLinks?.Invoke();
        }

        public void DestroyConnector() {
            if ( Parent ) {
                Parent.RemoveChild(this);
            }
            Destroy(gameObject);
        }

        void RemoveChild(Connector child) {
            Children.Remove(child);
            if ( Children.Count == 0 ) {
                DestroyConnector();
                OnOutOfLinks?.Invoke();
            }
        }
    }
}