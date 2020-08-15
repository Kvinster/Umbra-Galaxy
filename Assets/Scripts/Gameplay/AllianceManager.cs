using System;
using System.Collections.Generic;
using System.Linq;

namespace STP.Gameplay {
    public class AllianceManager {
        sealed class Alliance {
            public HashSet<ConflictSide> Sides = new HashSet<ConflictSide>();
            public bool Contains(ConflictSide side) {
                return Sides.Contains(side);
            }
        }
        
        List<Alliance> _alliances = new List<Alliance> {
            new Alliance {
                Sides = new HashSet<ConflictSide> {
                    ConflictSide.Player,
                    ConflictSide.Civilians
                }
            }
        };
        
        public HashSet<ConflictSide> GetAlliesSides(ConflictSide side) {
            var alliances = _alliances.FindAll((x) => x.Contains(side));
            var result = new HashSet<ConflictSide>();
            foreach ( var alliance in alliances ) {
                result.UnionWith(alliance.Sides);
            }
            return result;
        }
        
        public HashSet<ConflictSide> GetEnemiesSides(ConflictSide side) {
            var allSides = Enum.GetValues(typeof(ConflictSide)).Cast<ConflictSide>();
            var result = new HashSet<ConflictSide>(allSides);
            result.ExceptWith(GetAlliesSides(side));
            return result;
        }
    }
}