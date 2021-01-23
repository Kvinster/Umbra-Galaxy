using System;
using System.Collections.Generic;

using STP.Utils;

namespace STP.Behaviour.Core.Enemy {
    public class Connector : GameComponent {
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

        public void Init() {
            IsInit = true;
        }

        public void ForceDestroy() {
            foreach ( var link in Children ) {
                link.ForceDestroy();
            }
            Destroy(gameObject);
            OnOutOfLinks?.Invoke();
        }

        public void DestroyConnector() {
            Parent.RemoveChild(this);
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