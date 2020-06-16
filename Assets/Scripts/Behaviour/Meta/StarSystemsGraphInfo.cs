using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

using STP.Common;

namespace STP.Behaviour.Meta {
    [Serializable]
    public class StarSystemsGraphInfo {
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

        [SerializeField]
        List<StarSystemStartInfo> StarSystemStartInfos = new List<StarSystemStartInfo>();

        [SerializeField]
        List<StarSystemPair> StarSystemPairs = new List<StarSystemPair>();

        public List<string> StarSystems => StarSystemStartInfos.Select(x => x.Name).ToList();

        public List<StarSystemPair> GetStarSystemPairsInEditor() {
#if UNITY_EDITOR
            return StarSystemPairs;
#else
            return null;
#endif
        }

        public List<StarSystemStartInfo> GetStarSystemStartInfosInEditor() {
#if UNITY_EDITOR
            return StarSystemStartInfos;
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

        public List<string> GetNeighbouringStarSystems(string starSystemName) {
            var res = new List<string>();
            foreach ( var pair in StarSystemPairs ) {
                if ( (pair.A == starSystemName) && !res.Contains(pair.B) ) {
                    res.Add(pair.B);
                }
                if ( (pair.B == starSystemName) && !res.Contains(pair.A) ) {
                    res.Add(pair.A);
                } 
            }
            return res;
        }

        public Faction GetStarSystemFaction(string starSystemName) {
            return TryGetStarSystemStartInfo(starSystemName, out var startInfo) ? startInfo.Faction : Faction.Unknown;
        }

        public Sprite GetStarSystemPortrait(string starSystemName) {
            return TryGetStarSystemStartInfo(starSystemName, out var startInfo) ? startInfo.Portrait : null;
        }

        public List<StarSystemStartInfo> GetStarSystemStartInfos() {
            return StarSystemStartInfos;
        }

        public bool CheckValidity() {
            if ( StarSystemPairs.Count == 0 ) {
                Debug.LogError("StarSystemPairs.Count == 0");
                return false;
            }
            if ( StarSystemStartInfos.Count == 0 ) {
                Debug.LogError("StarSystemStartInfos.Count == 0");
                return false;
            }
            foreach ( var starSystemStartInfo in StarSystemStartInfos ) {
                if ( StarSystemStartInfos.Any(x =>
                    (x != starSystemStartInfo) && (x.Name == starSystemStartInfo.Name)) ) {
                    Debug.LogErrorFormat("Duplicate StarSystemStartInfo entries for '{0}'", starSystemStartInfo.Name);
                    return false;
                }
            }
            var starSystemNames = StarSystemStartInfos.Select(x => x.Name).ToList();
            foreach ( var pair in StarSystemPairs ) {
                if ( !starSystemNames.Contains(pair.A) ) {
                    Debug.LogErrorFormat(
                        "Unmentioned in StarSystemStartInfos star system '{0}' is used in StarSystemPairs", pair.A);
                    return false;
                }
                if ( !starSystemNames.Contains(pair.B) ) {
                    Debug.LogErrorFormat(
                        "Unmentioned in StarSystemStartInfos star system '{0}' is used in StarSystemPairs", pair.B);
                    return false;
                }
            }
            foreach ( var starSystemName in starSystemNames ) {
                var count = StarSystemPairs.Count(x => ((x.A == starSystemName) || (x.B == starSystemName))); 
                if ( count != starSystemNames.Count - 1 ) {
                    Debug.LogErrorFormat("Invalid number of pairs mentioning star system '{0}': '{1}'", starSystemName,
                        count);
                    return false;
                }
            }
            foreach ( var pair in StarSystemPairs ) {
                if ( !pair.CheckValidity() ) {
                    return false;
                }
            }
            foreach ( var startInfo in StarSystemStartInfos ) {
                if ( !startInfo.CheckValidity() ) {
                    return false;
                }
            }
            return true;
        }

        public StarSystemsGraphInfo Clone() {
            var newInfo = new StarSystemsGraphInfo();
            foreach ( var pair in StarSystemPairs ) {
                newInfo.StarSystemPairs.Add(pair.Clone());
            }
            foreach ( var startInfo in StarSystemStartInfos ) {
                newInfo.StarSystemStartInfos.Add(startInfo.Clone());
            }
            return newInfo;
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

        bool TryGetStarSystemStartInfo(string starSystemName, out StarSystemStartInfo startInfo) {
            foreach ( var tmpStartInfo in StarSystemStartInfos ) {
                if ( tmpStartInfo.Name == starSystemName ) {
                    startInfo = tmpStartInfo;
                    return true;
                }
            }
            Debug.LogErrorFormat("Can't find StarSystemStartInfo for star system '{0}'", starSystemName);
            startInfo = null;
            return false;
        }
    }
}
