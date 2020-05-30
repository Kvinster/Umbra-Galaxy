using UnityEngine;

using System.Collections.Generic;

namespace STP.Behaviour.Starter {
    public abstract class BaseGameComponent<T> : MonoBehaviour where T : BaseStarter<T> {
        public static readonly List<BaseGameComponent<T>> Instances = new List<BaseGameComponent<T>>();

        protected void OnEnable() {
            Instances.Add(this);
        }

        protected void OnDisable() {
            Instances.Remove(this);
        }

        public void Init(T starter) {
            InitInternal(starter);
        }

        protected abstract void InitInternal(T starter);
    }
}
