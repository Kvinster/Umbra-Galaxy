using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Meta;
using STP.Common;
using STP.Utils;

namespace STP.State {
    public sealed class StarSystemsController {
        static StarSystemsController _instance;
        public static StarSystemsController Instance => _instance ?? (_instance = new StarSystemsController().Init());

        StarSystemsGraphInfo _graphInfo;

        readonly List<StarSystemPath>             _cachedPaths      = new List<StarSystemPath>();
        readonly Dictionary<string, List<string>> _cachedNeighbours = new Dictionary<string, List<string>>();
        
        readonly List<string> _starSystemNames = new List<string>();
        
        readonly Dictionary<string, StarSystemState> _starSystemStates = new Dictionary<string, StarSystemState>();

        public event Action<string, int> OnStarSystemMoneyChanged;

        public bool HasStarSystem(string starSystemName) {
            return _starSystemNames.Contains(starSystemName);
        }

        public int GetStarSystemMoney(string starSystemName) {
            return TryGetStarSystemState(starSystemName, out var starSystemState) ? starSystemState.Money : -1;
        }

        public Faction GetStarSystemFaction(string starSystemName) {
            return TryGetStarSystemState(starSystemName, out var starSystemState)
                ? starSystemState.Faction
                : Faction.Unknown;
        }

        public bool TrySubStarSystemMoney(string starSystemName, int subMoney) {
            if ( !TryGetStarSystemState(starSystemName, out var starSystemState) ) {
                return false;
            }
            if ( starSystemState.Money >= subMoney ) {
                starSystemState.Money -= subMoney;
                OnStarSystemMoneyChanged?.Invoke(starSystemName, starSystemState.Money);
                return true;
            }
            return false;
        }

        public void AddStarSystemMoney(string starSystemName, int addMoney) {
            if ( !TryGetStarSystemState(starSystemName, out var starSystemState) ) {
                return;
            }
            starSystemState.Money += addMoney;
            OnStarSystemMoneyChanged?.Invoke(starSystemName, starSystemState.Money);
        }
        
        public int GetDistance(string aStarSystem, string bStarSystem) {
            var distance = _graphInfo.GetDistance(aStarSystem, bStarSystem); 
            return (distance == 0) ? (int.MaxValue / 2) : distance;
        }

        public Sprite GetStarSystemPortrait(string starSystemName) {
            return _graphInfo.GetStarSystemPortrait(starSystemName);
        }

        public StarSystemPath GetPath(string aStarSystem, string bStarSystem) {
            if ( !TryFindPath(aStarSystem, bStarSystem, out var path) ) {
                path = CalcPath(aStarSystem, bStarSystem);
                _cachedPaths.Add(path);
            }
            return path;
        }

        StarSystemsController Init() {
            var graphInfoScriptableObject =
                Resources.Load<StarSystemsGraphInfoScriptableObject>(StarSystemsGraphInfoScriptableObject
                    .ResourcesPath);
            if ( !graphInfoScriptableObject ) {
                Debug.LogError("Can't load StarSystemsGraphInfo");
                return null;
            }
            _graphInfo = graphInfoScriptableObject.StarSystemsGraphInfo.Clone();
            foreach ( var startInfo in _graphInfo.GetStarSystemStartInfos() ) {
                _starSystemNames.Add(startInfo.Name);
                _starSystemStates.Add(startInfo.Name,
                    new StarSystemState(startInfo.Name, startInfo.Faction, startInfo.StartMoney));
            }
            return this;
        }

        bool TryFindPath(string aStarSystemName, string bStarSystemName, out StarSystemPath path) {
            foreach ( var tmpPath in _cachedPaths ) {
                if ( ((tmpPath.StartStarSystemName == aStarSystemName) &&
                      (tmpPath.FinishStarSystemName == bStarSystemName)) ||
                     ((tmpPath.StartStarSystemName == bStarSystemName) &&
                      (tmpPath.FinishStarSystemName == aStarSystemName)) ) {
                    path = tmpPath;
                    return true;
                }
            }
            path = null;
            return false;
        }

        StarSystemPath CalcPath(string aStarSystemName, string bStarSystemName) {
            var (path, length) = DijkstraPathFinder.GetPath(aStarSystemName, bStarSystemName, _starSystemNames,
                GetDistance, GetNeighbouringStarSystems);
            return new StarSystemPath(path, length);
        }

        List<string> GetNeighbouringStarSystems(string starSystemName) {
            if ( !_cachedNeighbours.TryGetValue(starSystemName, out var neighbours) ) {
                neighbours = _graphInfo.GetNeighbouringStarSystems(starSystemName);
                _cachedNeighbours.Add(starSystemName, neighbours);
            }
            return neighbours;
        }

        bool TryGetStarSystemState(string starSystemName, out StarSystemState starSystemState) {
            if ( !_starSystemStates.TryGetValue(starSystemName, out starSystemState) ) {
                Debug.LogErrorFormat("Can't find state for '{0}'", starSystemName);
                return false;
            }
            return true;
        }
    }
}
