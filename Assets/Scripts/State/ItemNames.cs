using System.Collections.Generic;
using System.Linq;

namespace STP.State {
    public class ItemNames {
        public const string Mineral  = "mineral";
        public const string Scrap    = "scrap";
        public const string FuelTank = "fuel_tank";
        public const string MetaItem = "meta_item";
        
        public static readonly List<string> QuestItems = new List<string>{MetaItem};
        public static readonly List<string> UsualItems = new List<string>{Mineral, Scrap, FuelTank};
        public static readonly List<string> AllItems   = new List<string>(UsualItems.Union(QuestItems));
    }
}