using UnityEngine;

using System.Collections.Generic;

namespace STP.Behaviour.Starter {
    public abstract class BaseGameComponent<T> : MonoBehaviour where T : BaseStarter<T> {
        public static readonly List<BaseGameComponent<T>> Instances = new List<BaseGameComponent<T>>();
        
        public virtual bool HighPriorityInit => false;
        
        protected bool IsInit { get; private set; }

        protected void OnEnable() {
            if ( HighPriorityInit ) {
                Instances.Insert(0, this);
            } else {
                Instances.Add(this);
            }
        }

        protected void OnDisable() {
            Instances.Remove(this);
        }

        public void Init(T starter) {
            if ( IsInit ) {
                return;
            }
            InitInternal(starter);
            IsInit = true;
        }

        protected abstract void InitInternal(T starter);
    }
}
