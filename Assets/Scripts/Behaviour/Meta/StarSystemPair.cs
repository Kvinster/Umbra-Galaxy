using UnityEngine;

using System;

namespace STP.Behaviour.Meta {
    [Serializable]
    public sealed class StarSystemPair {
        public string A;
        public string B;
        public int    Distance = -1;

        public StarSystemPair Clone() {
            return new StarSystemPair {
                A        = A,
                B        = B,
                Distance = Distance
            };
        }

        public bool CheckValidity() {
            if ( string.IsNullOrEmpty(A) || string.IsNullOrEmpty(B) ) {
                Debug.LogErrorFormat("StarSystemPair: star system name is null or empty");
                return false;
            }
            if ( (Distance < 0) && (Distance != -1) ) {
                Debug.LogErrorFormat("StarSystemPair: invalid distance: '{0}'", Distance);
                return false;
            }
            return true;
        }

        bool Equals(StarSystemPair other) {
            return (A == other.A) && (B == other.B);
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
}
