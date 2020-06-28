using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Utils.PropertyAttribute;

namespace STP.Behaviour.Meta {
    [Serializable]
    public sealed class DarknessPath {
        [StarSystemName]
        public List<string> Path = new List<string>();

        public bool CheckValidity(StarSystemsGraphInfo graphInfo) {
            if ( Path.Count == 0 ) {
                Debug.LogError("Path is empty");
                return false;
            }
            var starSystems = graphInfo.StarSystems;
            foreach ( var starSystem in Path ) {
                if ( !starSystems.Contains(starSystem) ) {
                    Debug.LogErrorFormat("Unknown star system '{0}' used in the path", starSystem);
                    return false;
                }
            }
            return true;
        }
    }
}
