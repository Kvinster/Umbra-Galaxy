using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Utils;

using Cysharp.Threading.Tasks;
using DG.Tweening;
using Shapes;

namespace STP.Behaviour.Core.Enemy {
    public sealed class Connector : GameComponent {
        const float DieTime = 0.15f;

        public Connector       Parent;
        public List<Connector> Children;

        Tween _dieAnim;

        float _hp = 1f;

        public float Hp {
            get => _hp;
            private set {
                if ( Mathf.Approximately(_hp, value) ) {
                    return;
                }
                _hp = value;
                OnHpChanged?.Invoke(value);
            }
        }

        bool IsAlive => _dieAnim == null;

        public event Action        OnOutOfLinks;
        public event Action<float> OnHpChanged;
        public event Action<bool>  OnStartedDying; // from main connector?

        void Start() {
            foreach ( var line in GetComponentsInChildren<Line>() ) {
                if ( line.gameObject.name.StartsWith("Minimap") ) {
                    continue;
                }
                if ( !line.gameObject.GetComponent<ConnectorLine>() ) {
                    line.gameObject.AddComponent<ConnectorLine>();
                }
            }
        }

        public void StartForceDestroyConnector(bool fromConnector) {
            if ( !IsAlive ) {
                return;
            }
            transform.parent = null;
            UniTask.Void(() => ForceDestroyConnector(fromConnector));
            OnStartedDying?.Invoke(true);
        }

        public void StartDestroyConnector(bool fromConnector) {
            if ( !IsAlive ) {
                return;
            }
            transform.parent = null;
            UniTask.Void(() => DestroyConnector(fromConnector));
            OnStartedDying?.Invoke(false);
        }

        async UniTaskVoid ForceDestroyConnector(bool fromConnector) {
            if ( !IsAlive ) {
                return;
            }
            if ( fromConnector ) {
                await DieAnim(true);
            }
            foreach ( var link in Children ) {
                link.StartForceDestroyConnector(true);
            }
            Destroy(gameObject);
            OnOutOfLinks?.Invoke();
        }

        async UniTaskVoid DestroyConnector(bool fromConnector) {
            if ( !IsAlive ) {
                return;
            }
            await DieAnim(fromConnector);
            if ( Parent ) {
                Parent.RemoveChild(this);
            }
            Destroy(gameObject);
        }

        async UniTask DieAnim(bool fromConnector) {
            if ( !IsAlive ) {
                return;
            }
            _dieAnim = DOTween.To(() => Hp, x => Hp = x, 0f, DieTime)
                .SetEase(fromConnector ? Ease.Linear : Ease.InSine);
            await _dieAnim;
        }

        void RemoveChild(Connector child) {
            Children.Remove(child);
            if ( Children.Count == 0 ) {
                StartDestroyConnector(true);
                OnOutOfLinks?.Invoke();
            }
        }
    }
}