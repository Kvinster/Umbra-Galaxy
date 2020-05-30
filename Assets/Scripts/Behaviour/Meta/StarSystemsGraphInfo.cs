using UnityEngine;

using System;
using System.Collections.Generic;

namespace STP.Behaviour.Meta {
    [CreateAssetMenu(menuName = "Create StarSystemsGraphInfo", fileName = "StarSystems")]
    public sealed class StarSystemsGraphInfo : ScriptableObject {
        [Serializable]
        public sealed class StarSystemPair {
            public string A;
            public string B;
            public int    Distance;

            bool Equals(StarSystemPair other) {
                return A == other.A && B == other.B;
            }

            public override bool Equals(object obj) {
                return ReferenceEquals(this, obj) || obj is StarSystemPair other && Equals(other);
            }

            public override int GetHashCode() {
                unchecked {
                    return ((A != null ? A.GetHashCode() : 0) * 397) ^ (B != null ? B.GetHashCode() : 0);
                }
            }
        }

        [HideInInspector]
        public List<StarSystemPair> StarSystemPairs = new List<StarSystemPair>();
    }
}
