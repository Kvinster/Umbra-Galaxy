using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

using STP.Behaviour.Meta;
using STP.Common;
using STP.Utils;

namespace STP.State.Meta {
    public sealed class StarSystemsController : BaseStateController {
        StarSystemsGraphInfo _graphInfo;
        
        readonly List<string> _starSystemIds = new List<string>();

        readonly Dictionary<string, FactionSystemState> _factionSystemStates =
            new Dictionary<string, FactionSystemState>();

        readonly Dictionary<string, ShardSystemState> _shardSystemStates = new Dictionary<string, ShardSystemState>();

        public event Action<string, int>  OnStarSystemMoneyChanged;
        public event Action<string, bool> OnStarSystemActiveChanged;
        public event Action<string, int>  OnStarSystemSurvivalChanceChanged;

        public override void Init() {
            var graphInfoScriptableObject =
                Resources.Load<StarSystemsGraphInfoScriptableObject>(StarSystemsGraphInfoScriptableObject
                    .ResourcesPath);
            if ( !graphInfoScriptableObject ) {
                Debug.LogError("Can't load StarSystemsGraphInfo");
                return;
            }
            _graphInfo = graphInfoScriptableObject.StarSystemsGraphInfo.Clone();
            foreach ( var factionSystemInfo in _graphInfo.FactionSystemInfos ) {
                _starSystemIds.Add(factionSystemInfo.Id);
                _factionSystemStates.Add(factionSystemInfo.Id,
                    new FactionSystemState(factionSystemInfo.Id, factionSystemInfo.Name, factionSystemInfo.Faction,
                        factionSystemInfo.StartMoney, factionSystemInfo.BaseSurvivalChance));
            }
            foreach ( var shardInfo in _graphInfo.ShardSystemInfos ) {
                _starSystemIds.Add(shardInfo.Id);
                // TODO: set actual isActive
                _shardSystemStates.Add(shardInfo.Id, new ShardSystemState(shardInfo.Id, true));
            }
        }

        public bool HasStarSystem(string starSystemId) {
            return _starSystemIds.Contains(starSystemId);
        }

        public int GetDistance(string aStarSystemId, string bStarSystemId) {
            var distance = _graphInfo.GetDistance(aStarSystemId, bStarSystemId); 
            return (distance == 0) ? (int.MaxValue / 2) : distance;
        }
        
        public List<string> GetNeighbourStarSystemIds(string starSystemId) {
            return _graphInfo.GetNeighbouringStarSystems(starSystemId);
        }

        public string GetStarSystemName(string starSystemId) {
            return TryGetFactionSystemState(starSystemId, out var starSystemState, true)
                ? starSystemState.Name
                : _graphInfo.GetStarSystemName(starSystemId);
        }

        public List<string> GetFactionSystemIds() {
            return _factionSystemStates.Select(x => x.Key).ToList();
        }

        public int GetFactionSystemMoney(string starSystemId) {
            return TryGetFactionSystemState(starSystemId, out var starSystemState) ? starSystemState.Money : -1;
        }

        public Faction GetFactionSystemFaction(string starSystemId) {
            return TryGetFactionSystemState(starSystemId, out var starSystemState)
                ? starSystemState.Faction
                : Faction.Unknown;
        }

        public bool GetFactionSystemActive(string starSystemId) {
            return TryGetFactionSystemState(starSystemId, out var starSystemState) && starSystemState.IsActive;
        }

        public void SetFactionSystemActive(string starSystemId, bool isActive) {
            if ( TryGetFactionSystemState(starSystemId, out var starSystemState) &&
                 (starSystemState.IsActive != isActive) ) {
                starSystemState.IsActive = isActive;
                OnStarSystemActiveChanged?.Invoke(starSystemId, isActive);
            }
        }

        public int GetFactionSystemSurvivalChance(string starSystemId) {
            return TryGetFactionSystemState(starSystemId, out var starSystemState) ? starSystemState.SurvivalChance : -1;
        }

        public void AddFactionSystemSurvivalChance(string starSystemId, int addSurvivalChance) {
            if ( addSurvivalChance == 0 ) {
                return;
            }
            if ( TryGetFactionSystemState(starSystemId, out var factionSystemState) ) {
                factionSystemState.SurvivalChance += addSurvivalChance;
                OnStarSystemSurvivalChanceChanged?.Invoke(starSystemId, factionSystemState.SurvivalChance);
            }
        }

        public bool TrySubFactionSystemMoney(string starSystemId, int subMoney) {
            if ( !TryGetFactionSystemState(starSystemId, out var starSystemState) ) {
                return false;
            }
            if ( starSystemState.Money >= subMoney ) {
                starSystemState.Money -= subMoney;
                OnStarSystemMoneyChanged?.Invoke(starSystemId, starSystemState.Money);
                return true;
            }
            return false;
        }

        public void AddFactionSystemMoney(string starSystemId, int addMoney) {
            if ( !TryGetFactionSystemState(starSystemId, out var starSystemState) ) {
                return;
            }
            starSystemState.Money += addMoney;
            OnStarSystemMoneyChanged?.Invoke(starSystemId, starSystemState.Money);
        }

        public Sprite GetFactionSystemPortrait(string starSystemId) {
            return _graphInfo.GetStarSystemPortrait(starSystemId);
        }

        public List<string> GetActiveShardSystemIds() {
            return _graphInfo.ShardSystemInfos.Where(x => GetShardSystemActive(x.Id)).Select(x => x.Id).ToList();
        }

        public bool GetShardSystemActive(string starSystemId) {
            return TryGetShardSystemState(starSystemId, out var shardSystemState) && shardSystemState.IsActive;
        }

        public void SetShardSystemActive(string starSystemId, bool isActive) {
            if ( TryGetShardSystemState(starSystemId, out var shardSystemState) ) {
                if ( shardSystemState.IsActive == isActive ) {
                    return;
                }
                shardSystemState.IsActive = isActive;
                OnStarSystemActiveChanged?.Invoke(starSystemId, isActive);
            }
        }

        public StarSystemPath GetPath(string aStarSystemId, string bStarSystemId) {
            var path = CalcPath(aStarSystemId, bStarSystemId);
            if ( (path.StartStarSystemId == aStarSystemId) && (path.FinishStarSystemId == bStarSystemId) ) {
                return path;
            }
            if ( (path.StartStarSystemId == bStarSystemId) && (path.FinishStarSystemId == aStarSystemId) ) {
                return path.Reversed();
            }
            Debug.LogError("Unsupported scenario");
            return null;
        }

        public StarSystemType GetStarSystemType(string starSystemId) {
            foreach ( var factionSystemInfo in _graphInfo.FactionSystemInfos ) {
                if ( factionSystemInfo.Id == starSystemId ) {
                    return StarSystemType.Faction;
                }
            }
            foreach ( var shardInfo in _graphInfo.ShardSystemInfos ) {
                if ( shardInfo.Id == starSystemId ) {
                    return StarSystemType.Shard;
                }
            }
            Debug.LogErrorFormat("Can't find star system info for id '{0}'", starSystemId);
            return StarSystemType.Unknown;
        }

        StarSystemPath CalcPath(string aStarSystemId, string bStarSystemId) {
            var (path, length) = DijkstraPathFinder.GetPath(aStarSystemId, bStarSystemId, _starSystemIds,
                GetDistance, GetNeighbouringStarSystems);
            return new StarSystemPath(path, length);
        }

        List<string> GetNeighbouringStarSystems(string startStarSystemId, string starSystemId) {
            var neighbours = _graphInfo.GetNeighbouringStarSystems(starSystemId);
            for ( var i = neighbours.Count - 1; i >= 0; i-- ) {
                var neighbourId   = neighbours[i];
                if ( neighbourId == startStarSystemId ) {
                    continue;
                }
                var neighbourType = GetStarSystemType(neighbourId);
                switch ( neighbourType ) {
                    case StarSystemType.Faction: {
                        if ( !TryGetFactionSystemState(neighbourId, out _) ) {
                            neighbours.RemoveAt(i);
                        }
                        break;
                    }
                    case StarSystemType.Shard: {
                        if ( !TryGetShardSystemState(neighbourId, out var shardSystemState) ) {
                            neighbours.RemoveAt(i);
                            break;
                        }
                        if ( !shardSystemState.IsActive ) {
                            neighbours.RemoveAt(i);
                        }
                        break;
                    }
                    default: {
                        Debug.LogErrorFormat("Unsupported system type '{0}'", neighbourType.ToString());
                        neighbours.RemoveAt(i);
                        break;
                    }
                }
            }
            return neighbours;
        }

        bool TryGetFactionSystemState(string starSystemId, out FactionSystemState factionSystemState,
            bool silent = false) {
            if ( !_factionSystemStates.TryGetValue(starSystemId, out factionSystemState) ) {
                if ( !silent ) {
                    Debug.LogErrorFormat("Can't find state for '{0}'", starSystemId);
                }
                return false;
            }
            return true;
        }

        bool TryGetShardSystemState(string starSystemId, out ShardSystemState shardSystemState, bool silent = false) {
            if ( !_shardSystemStates.TryGetValue(starSystemId, out shardSystemState) ) {
                if ( !silent ) {
                    Debug.LogErrorFormat("Can't find state for '{0}'", starSystemId);
                }
                return false;
            }
            return true;
        }
    }
}
