using UnityEngine;

using System;

using STP.Common;

namespace STP.Behaviour.Meta {
    [Serializable]
    public sealed class FactionStarSystemInfo {
        public string  Id; 
        public string  Name;
        public Faction Faction;
        public int     StartMoney;
        public int     BaseSurvivalChance;
        public Sprite  Portrait;

        public FactionStarSystemInfo Clone() {
            return new FactionStarSystemInfo {
                Id                 = Id,
                Name               = Name,
                Faction            = Faction,
                StartMoney         = StartMoney,
                BaseSurvivalChance = BaseSurvivalChance,
                Portrait           = Portrait
            };
        }

        public bool CheckValidity() {
            if ( string.IsNullOrEmpty(Id) ) {
                Debug.LogErrorFormat("FactionStarSystemInfo: Id is null or empty");
                return false;
            }
            if ( string.IsNullOrEmpty(Name) ) {
                Debug.LogErrorFormat("FactionStarSystemInfo: Name is null or empty");
                return false;
            }
            if ( Faction == Faction.Unknown ) {
                Debug.LogErrorFormat("FactionStarSystemInfo: Faction is unknown");
                return false;
            }
            if ( StartMoney < 0 ) {
                Debug.LogErrorFormat("FactionStarSystemInfo: StartMoney is less than zero");
                return false;
            }
            return true;
        }
    }
}
