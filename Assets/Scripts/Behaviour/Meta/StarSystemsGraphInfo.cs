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

        [SerializeField]
        List<StarSystemPair> StarSystemPairs = new List<StarSystemPair>();

        public List<StarSystemPair> GetStarSystemPairsInEditor() {
#if UNITY_EDITOR
            return StarSystemPairs;
#else
            return null;
#endif
        }
        
        public int GetDistance(string aStarSystem, string bStarSystem) {
            if ( aStarSystem == bStarSystem ) {
                return 0;
            }
            return GetPair(aStarSystem, bStarSystem)?.Distance ?? -1;
        }

        StarSystemPair GetPair(string aStarSystem, string bStarSystem) {
            if ( aStarSystem == bStarSystem ) {
                return null;
            }
            foreach ( var pair in StarSystemPairs ) {
                if ( ((pair.A == aStarSystem) && (pair.B == bStarSystem)) ||
                     ((pair.A == bStarSystem) && (pair.B == aStarSystem)) ) {
                    return pair;
                }
            }
            Debug.LogErrorFormat("Can't find pair for ('{0}', '{1}')", aStarSystem, bStarSystem);
            return null;
        }
    }
}
