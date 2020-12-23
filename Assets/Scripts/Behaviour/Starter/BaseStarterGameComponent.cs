using System.Collections.Generic;

using STP.Utils;

namespace STP.Behaviour.Starter {
    public abstract class BaseStarterGameComponent<T> : GameComponent where T : BaseStarter<T> {
        public static readonly List<BaseStarterGameComponent<T>> Instances = new List<BaseStarterGameComponent<T>>();
        
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
