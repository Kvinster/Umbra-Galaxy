using UnityEngine;

namespace STP.Utils {
    public abstract class GameComponent : MonoBehaviour{
        public void Awake()      => CheckDescription();
        public void OnValidate() => CheckDescription();
        
        protected abstract void CheckDescription();
    }
}