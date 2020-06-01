using UnityEngine;

namespace STP.Utils {
    public abstract class GameBehaviour : MonoBehaviour{
        public void Awake()      => CheckDescription();
        public void OnValidate() => CheckDescription();
        
        protected abstract void CheckDescription();
    }
}