using System.Collections.Generic;

using STP.Utils;

namespace STP.Behaviour.Starter {
    public abstract class BaseStarter : GameComponent {
    }

    public abstract class BaseStarter<T> : BaseStarter where T : BaseStarter<T> {
        protected void InitComponents() {
            var comps   = new List<BaseStarterGameComponent<T>>(BaseStarterGameComponent<T>.Instances);
            var starter = this as T;
            foreach ( var comp in comps ) {
                comp.Init(starter);
            }
        }
    }
}
