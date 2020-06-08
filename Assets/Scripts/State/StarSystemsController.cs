using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Meta;
using STP.Common;

namespace STP.State {
    public sealed class StarSystemsController {
        static StarSystemsController _instance;
        public static StarSystemsController Instance => _instance ?? (_instance = new StarSystemsController().Init());

        StarSystemsGraphInfo _graphInfo;
        
        readonly Dictionary<string, StarSystemState> _starSystemStates = new Dictionary<string, StarSystemState>();

        public event Action<string, int> OnStarSystemMoneyChanged;

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
            return _graphInfo.GetDistance(aStarSystem, bStarSystem);
        }

        public Sprite GetStarSystemPortrait(string starSystemName) {
            return _graphInfo.GetStarSystemPortrait(starSystemName);
        }

        StarSystemsController Init() {
            _graphInfo = Resources.Load<StarSystemsGraphInfo>("Meta/StarSystems");
            if ( !_graphInfo ) {
                Debug.LogError("Can't load StarSystemsGraphInfo");
                return null;
            }
            foreach ( var startInfo in _graphInfo.GetStarSystemStartInfos() ) {
                _starSystemStates.Add(startInfo.Name,
                    new StarSystemState(startInfo.Name, startInfo.Faction, startInfo.StartMoney));
            }
            return this;
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
