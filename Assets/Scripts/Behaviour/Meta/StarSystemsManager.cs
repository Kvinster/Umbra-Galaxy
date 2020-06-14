using UnityEngine;

using System.Collections.Generic;

using STP.State;

namespace STP.Behaviour.Meta {
    public sealed class StarSystemsManager {
        readonly Dictionary<string, BaseStarSystem> _starSystems = new Dictionary<string, BaseStarSystem>();

        public void RegisterStarSystem(BaseStarSystem starSystem) {
            if ( !StarSystemsController.Instance.HasStarSystem(starSystem.Name) ) {
                Debug.LogErrorFormat("Can't register star system '{0}'", starSystem.Name);
                return;
            }
            if ( _starSystems.ContainsKey(starSystem.Name) ) {
                Debug.LogErrorFormat("Star system '{0}' is already registered", starSystem.Name);
                return;
            }
            _starSystems.Add(starSystem.Name, starSystem);
        }

        public BaseStarSystem GetStarSystem(string starSystemName) {
            if ( !_starSystems.TryGetValue(starSystemName, out var starSystem) ) {
                Debug.LogErrorFormat("No registered star system '{0}'", starSystemName);
                return null;
            }
            return starSystem;
        }
    }
}
