using UnityEngine;

using System.Collections.Generic;

using STP.State;

namespace STP.Behaviour.Meta {
    public sealed class StarSystemsManager {
        readonly Dictionary<string, BaseStarSystem> _starSystems = new Dictionary<string, BaseStarSystem>();

        public void RegisterStarSystem(BaseStarSystem starSystemId) {
            if ( !StarSystemsController.Instance.HasStarSystem(starSystemId.Id) ) {
                Debug.LogErrorFormat("Can't register star system '{0}'", starSystemId.Id);
                return;
            }
            if ( _starSystems.ContainsKey(starSystemId.Id) ) {
                Debug.LogErrorFormat("Star system '{0}' is already registered", starSystemId.Id);
                return;
            }
            _starSystems.Add(starSystemId.Id, starSystemId);
        }

        public BaseStarSystem GetStarSystem(string starSystemId) {
            if ( !_starSystems.TryGetValue(starSystemId, out var starSystem) ) {
                Debug.LogErrorFormat("No registered star system '{0}'", starSystemId);
                return null;
            }
            return starSystem;
        }
    }
}
