using System.Collections.Generic;
using System.Linq;

namespace STP.State {
    public static class ItemNames {
        public const string Mineral  = "mineral";
        public const string Scrap    = "scrap";
        public const string FuelTank = "fuel_tank";
        
        public const string MetaItem = "meta_item";
        
        // Unique items for Delivery quests
        public const string RelicA = "relic_a";

        public static readonly List<string> UsualItems  = new List<string> { Mineral, Scrap, FuelTank };
        public static readonly List<string> QuestItems  = new List<string> { MetaItem };
        public static readonly List<string> UniqueItems = new List<string> { RelicA };
        public static readonly List<string> AllItems    = new List<string>(UsualItems.Union(QuestItems).Union(UniqueItems));
    }
}