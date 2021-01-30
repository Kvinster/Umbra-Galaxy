using UnityEngine;
using UnityEngine.VFX;

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
                var ve = line.gameObject.GetComponentInChildren<VisualEffect>();
                if ( ve ) {
                    var halfThickness = line.Thickness / 2f;
                    var isHor         = Mathf.Approximately(line.Start.y, line.End.y);
                    var start = line.Start + (isHor ? new Vector3(0, -halfThickness) : new Vector3(-halfThickness, 0));
                    var end   = line.End + (isHor ? new Vector3(0, halfThickness) : new Vector3(halfThickness, 0));
                    ve.SetVector3("Start", start);
                    ve.SetVector3("End", end);
                }
            }
        }

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