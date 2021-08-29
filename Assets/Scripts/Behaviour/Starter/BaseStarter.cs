using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Utils;

namespace STP.Behaviour.Starter {
    public abstract class BaseStarter : GameComponent { }

    public abstract class BaseStarter<T> : BaseStarter where T : BaseStarter<T> {
        protected void InitComponents() {
            var comps   = new List<BaseGameComponent<T>>(BaseGameComponent<T>.Instances);
            var starter = this as T;
            foreach ( var comp in comps ) {
                try {
                    comp.Init(starter);
                } catch ( Exception e ) {
                    Debug.LogErrorFormat("{0}.{1}: exception when initializing {2}", nameof(BaseStarter),
                        nameof(InitComponents), comp.GetType().Name);
                    Debug.LogException(e);
                }
            }
        }
    }
}
