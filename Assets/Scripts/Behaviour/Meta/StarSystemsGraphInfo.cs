using UnityEngine;
using UnityEngine.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;

using STP.Common;

namespace STP.Behaviour.Meta {
    [Serializable]
    public class StarSystemsGraphInfo {
        public List<StarSystemPair>        StarSystemPairs    = new List<StarSystemPair>();
        [FormerlySerializedAs("StarSystemStartInfos")]
        public List<FactionStarSystemInfo> FactionSystemInfos = new List<FactionStarSystemInfo>();
        public List<ShardStarSystemInfo>   ShardSystemInfos   = new List<ShardStarSystemInfo>();

        public List<string> FactionStarSystemsIds => FactionSystemInfos.Select(x => x.Id).ToList();
        public List<string> ShardSystemsIds       => ShardSystemInfos.Select(x => x.Id).ToList();
        public List<string> AllStarSystemsIds     => FactionStarSystemsIds.Concat(ShardSystemsIds).ToList(); 

        public int GetDistance(string aStarSystemId, string bStarSystemId) {
            if ( aStarSystemId == bStarSystemId ) {
                return 0;
            }
            return GetPair(aStarSystemId, bStarSystemId)?.Distance ?? -1;
        }
        
        public void SetDistance(string aStarSystemId, string bStarSystemId, int distance) {
            var pair = GetPair(aStarSystemId, bStarSystemId);
            if ( pair != null ) {
                pair.Distance = distance;
            }
        }

        public Faction GetFaction(string starSystemId) {
            return TryGetFactionSystemInfo(starSystemId, out var systemInfo)
                ? systemInfo.Faction
                : Faction.Unknown;
        }

        public void SetFaction(string starSystemId, Faction faction) {
            if ( TryGetFactionSystemInfo(starSystemId, out var systemInfo) ) {
                systemInfo.Faction = faction;
            }
        }

        public int GetMoney(string starSystemId) {
            return TryGetFactionSystemInfo(starSystemId, out var systemInfo)
                ? systemInfo.StartMoney
                : -1;
        }

        public void SetMoney(string starSystemId, int money) {
            if ( money < 0 ) {
                Debug.LogError("New start money for system '{0}' is less than zero");
                return;
            }
            if ( TryGetFactionSystemInfo(starSystemId, out var systemInfo) ) {
                systemInfo.StartMoney = money;
            }
        }

        public int GetSurvivalChance(string starSystemId) {
            return TryGetFactionSystemInfo(starSystemId, out var systemInfo)
                ? systemInfo.BaseSurvivalChance
                : -1;
        }

        public void SetSurvivalChance(string starSystemId, int survivalChance) {
            if ( TryGetFactionSystemInfo(starSystemId, out var systemInfo) ) {
                systemInfo.BaseSurvivalChance = survivalChance;
            }
        }

        public Sprite GetPortrait(string starSystemId) {
            return TryGetFactionSystemInfo(starSystemId, out var systemInfo)
                ? systemInfo.Portrait
                : null;
        }

        public void SetPortrait(string starSystemId, Sprite portrait) {
            if ( TryGetFactionSystemInfo(starSystemId, out var systemInfo) ) {
                systemInfo.Portrait = portrait;
            }
        }

        public List<string> GetNeighbouringStarSystems(string starSystemId) {
            var res = new List<string>();
            foreach ( var pair in StarSystemPairs ) {
                if ( pair.Distance <= 0 ) {
                    continue;
                }
                if ( (pair.A == starSystemId) && !res.Contains(pair.B) ) {
                    res.Add(pair.B);
                }
                if ( (pair.B == starSystemId) && !res.Contains(pair.A) ) {
                    res.Add(pair.A);
                } 
            }
            return res;
        }

        public string GetStarSystemName(string starSystemId) {
            if ( TryGetFactionSystemInfo(starSystemId, out var factionStarSystemInfo, true) ) {
                return factionStarSystemInfo.Name;
            } else if ( TryGetShardSystemInfo(starSystemId, out var shardStarSystemInfo, true) ) {
                return shardStarSystemInfo.Name;
            }
            Debug.LogErrorFormat("Can't find star system info for id '{0}'", starSystemId);
            return null;
        }

        public string GetFactionSystemName(string starSystemId) {
            return TryGetFactionSystemInfo(starSystemId, out var systemInfo) ? systemInfo.Name : null;
        }

        public string GetShardSystemName(string starSystemId) {
            return TryGetShardSystemInfo(starSystemId, out var shardInfo) ? shardInfo.Name : null;
        }

        public Faction GetStarSystemFaction(string starSystemId) {
            return TryGetFactionSystemInfo(starSystemId, out var startInfo) ? startInfo.Faction : Faction.Unknown;
        }

        public Sprite GetStarSystemPortrait(string starSystemId) {
            return TryGetFactionSystemInfo(starSystemId, out var startInfo) ? startInfo.Portrait : null;
        }

        public bool CheckValidity() {
            if ( StarSystemPairs.Count == 0 ) {
                Debug.LogError("StarSystemPairs.Count == 0");
                return false;
            }
            if ( FactionSystemInfos.Count == 0 ) {
                Debug.LogError("StarSystemStartInfos.Count == 0");
                return false;
            }
            foreach ( var starSystemStartInfo in FactionSystemInfos ) {
                if ( FactionSystemInfos.Any(x =>
                    (x != starSystemStartInfo) && (x.Id == starSystemStartInfo.Id)) ) {
                    Debug.LogErrorFormat("Duplicate StarSystemStartInfo entries for '{0}'", starSystemStartInfo.Id);
                    return false;
                }
            }
            var starSystemIds = FactionSystemInfos.Select(x => x.Id).ToList();
            foreach ( var pair in StarSystemPairs ) {
                if ( !starSystemIds.Contains(pair.A) ) {
                    Debug.LogErrorFormat(
                        "Unmentioned in StarSystemStartInfos star system '{0}' is used in StarSystemPairs", pair.A);
                    return false;
                }
                if ( !starSystemIds.Contains(pair.B) ) {
                    Debug.LogErrorFormat(
                        "Unmentioned in StarSystemStartInfos star system '{0}' is used in StarSystemPairs", pair.B);
                    return false;
                }
            }
            foreach ( var starSystemName in starSystemIds ) {
                var count = StarSystemPairs.Count(x => ((x.A == starSystemName) || (x.B == starSystemName))); 
                if ( count != starSystemIds.Count - 1 ) {
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
            foreach ( var factionSystemInfo in FactionSystemInfos ) {
                if ( !factionSystemInfo.CheckValidity() ) {
                    return false;
                }
            }
            foreach ( var shardInfo in ShardSystemInfos ) {
                if ( !shardInfo.CheckValidity() ) {
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
            foreach ( var startInfo in FactionSystemInfos ) {
                newInfo.FactionSystemInfos.Add(startInfo.Clone());
            }
            foreach ( var shardInfo in ShardSystemInfos ) {
                newInfo.ShardSystemInfos.Add(shardInfo.Clone());
            }
            return newInfo;
        }

        public StarSystemPair GetPair(string aStarSystemId, string bStarSystemId) {
            if ( aStarSystemId == bStarSystemId ) {
                return null;
            }
            foreach ( var pair in StarSystemPairs ) {
                if ( ((pair.A == aStarSystemId) && (pair.B == bStarSystemId)) ||
                     ((pair.A == bStarSystemId) && (pair.B == aStarSystemId)) ) {
                    return pair;
                }
            }
            Debug.LogErrorFormat("Can't find pair for ('{0}', '{1}')", aStarSystemId, bStarSystemId);
            return null;
        }

        public bool TryGetFactionSystemInfo(string starSystemId, out FactionStarSystemInfo info, bool silent = false) {
            foreach ( var tmpStartInfo in FactionSystemInfos ) {
                if ( tmpStartInfo.Id == starSystemId ) {
                    info = tmpStartInfo;
                    return true;
                }
            }
            if ( !silent ) {
                Debug.LogErrorFormat("Can't find FactionStarSystemInfo for star system '{0}'", starSystemId);
            }
            info = null;
            return false;
        }

        public bool TryGetShardSystemInfo(string starSystemId, out ShardStarSystemInfo info, bool silent = false) {
            foreach ( var tmpStartInfo in ShardSystemInfos ) {
                if ( tmpStartInfo.Id == starSystemId ) {
                    info = tmpStartInfo;
                    return true;
                }
            }
            if ( !silent ) {
                Debug.LogErrorFormat("Can't find ShardSystemInfo for star system '{0}'", starSystemId);
            }
            info = null;
            return false;
        }

        public void AddFactionStarSystem() {
            var newSystem = new FactionStarSystemInfo {
                Id                 = Guid.NewGuid().ToString(),
                Name               = string.Empty,
                BaseSurvivalChance = 50,
                Faction            = Faction.Unknown,
                StartMoney         = 0,
                Portrait           = null,
            };
            FactionSystemInfos.Add(newSystem);
            foreach ( var starSystemId in AllStarSystemsIds ) {
                if ( starSystemId == newSystem.Id ) {
                    continue;
                }
                StarSystemPairs.Add(new StarSystemPair {
                    A        = starSystemId,
                    B        = newSystem.Id,
                    Distance = 0
                });
            } 
        }

        public void RemoveFactionStarSystem(string starSystemId) {
            for ( var i = 0; i < FactionSystemInfos.Count; i++ ) {
                var starSystemInfo = FactionSystemInfos[i];
                if ( starSystemInfo.Id == starSystemId ) {
                    FactionSystemInfos.RemoveAt(i);
                    break;
                }
            }
            for ( var i = StarSystemPairs.Count - 1; i >= 0; i-- ) {
                var pair = StarSystemPairs[i];
                if ( (pair.A == starSystemId) || (pair.B == starSystemId) ) {
                    StarSystemPairs.RemoveAt(i);
                }
            }
            Debug.Assert(CheckValidity(), "Graph is now invalid");
        }

        public void AddShardStarSystem() {
            var newSystem = new ShardStarSystemInfo {
                Id   = Guid.NewGuid().ToString(),
                Name = string.Empty,
            };
            ShardSystemInfos.Add(newSystem);
            foreach ( var starSystemId in AllStarSystemsIds ) {
                if ( starSystemId == newSystem.Id ) {
                    continue;
                }
                StarSystemPairs.Add(new StarSystemPair {
                    A        = starSystemId,
                    B        = newSystem.Id,
                    Distance = 0
                });
            } 
        }

        public void RemoveShardStarSystem(string starSystemId) {
            for ( var i = 0; i < ShardSystemInfos.Count; i++ ) {
                var starSystemInfo = ShardSystemInfos[i];
                if ( starSystemInfo.Id == starSystemId ) {
                    ShardSystemInfos.RemoveAt(i);
                    break;
                }
            }
            for ( var i = StarSystemPairs.Count - 1; i >= 0; i-- ) {
                var pair = StarSystemPairs[i];
                if ( (pair.A == starSystemId) || (pair.B == starSystemId) ) {
                    StarSystemPairs.RemoveAt(i);
                }
            }
            Debug.Assert(CheckValidity(), "Graph is now invalid");
        }
    }
}
