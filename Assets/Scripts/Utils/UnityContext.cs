using UnityEngine;

using System;
using System.Collections.Generic;

namespace STP.Utils {
    public class UnityContext : SingleBehaviour<UnityContext> {
        readonly List<Action<float>> _updateCallbacks = new List<Action<float>>();
        
        public void AddUpdateCallback(Action<float> callback) {
            if ( callback == null ) {
                return;
            }
            _updateCallbacks.Add(callback);
        }

        public void RemoveUpdateCallback(Action<float> callback) {
            if ( callback == null ) {
                return;
            }
            _updateCallbacks.Remove(callback);
        }

        void Update() {
            var collectionCopy = new List<Action<float>>(_updateCallbacks);
            foreach ( var callback in collectionCopy ) {
               callback(Time.deltaTime); 
            }
        }
    }
}