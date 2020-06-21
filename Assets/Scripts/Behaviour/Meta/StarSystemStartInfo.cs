using UnityEngine;

using System;

using STP.Common;

namespace STP.Behaviour.Meta {
    [Serializable]
    public sealed class StarSystemStartInfo {
        public string  Name;
        public Faction Faction;
        public int     StartMoney;
        public Sprite  Portrait;

        public StarSystemStartInfo Clone() {
            return new StarSystemStartInfo {
                Name       = Name,
                Faction    = Faction,
                StartMoney = StartMoney,
                Portrait   = Portrait
            };
        }

        public bool CheckValidity() {
            if ( string.IsNullOrEmpty(Name) ) {
                Debug.LogErrorFormat("StarSystemStartInfo: Name is null or empty");
                return false;
            }
            if ( Faction == Faction.Unknown ) {
                Debug.LogErrorFormat("StarSystemStartInfo: Faction is unknown");
                return false;
            }
            if ( StartMoney < 0 ) {
                Debug.LogErrorFormat("StarSystemStartInfo: StartMoney is less than zero");
                return false;
            }
            return true;
        }
    }
}
