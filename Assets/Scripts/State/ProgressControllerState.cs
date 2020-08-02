using System.Collections.Generic;

using STP.Common;

namespace STP.State {
    public sealed class ProgressControllerState {
        public Dictionary<Faction, int> UberArtifacts = new Dictionary<Faction, int> {
            { Faction.A, 0 },
            { Faction.B, 0 },
            { Faction.C, 0 }
        };
    }
}
