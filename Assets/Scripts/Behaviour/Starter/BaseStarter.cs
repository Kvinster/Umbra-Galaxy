using UnityEngine;

using System.Collections.Generic;

namespace STP.Behaviour.Starter {
    public abstract class BaseStarter : MonoBehaviour {
    }

    public abstract class BaseStarter<T> : BaseStarter where T : BaseStarter<T> {
        protected void InitComponents() {
            var comps   = new List<BaseGameComponent<T>>(BaseGameComponent<T>.Instances);
            var starter = this as T;
            foreach ( var comp in comps ) {
                comp.Init(starter);
            }
        }
    }
}
