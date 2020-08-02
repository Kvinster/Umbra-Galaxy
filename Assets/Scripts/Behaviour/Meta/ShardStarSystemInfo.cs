using UnityEngine;

using System;

namespace STP.Behaviour.Meta {
    [Serializable]
    public sealed class ShardStarSystemInfo {
        public string Id;
        public string Name;
        public int    Level;

        public ShardStarSystemInfo Clone() {
            return new ShardStarSystemInfo {
                Id    = Id,
                Name  = Name,
                Level = Level
            };
        }

        public bool CheckValidity() {
            if ( string.IsNullOrEmpty(Id) ) {
                Debug.LogError("Id is null or empty");
                return false;
            }
            if ( string.IsNullOrEmpty(Name) ) {
                Debug.LogError("Name is null or empty");
                return false;
            }
            if ( Level < 0 ) {
                Debug.LogErrorFormat("Level is below zero '{0}'", Level);
                return false;
            }
            return true;
        }
    }
}
