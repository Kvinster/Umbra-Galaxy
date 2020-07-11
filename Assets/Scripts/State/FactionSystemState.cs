using STP.Common;

namespace STP.State {
    public sealed class FactionSystemState {
        public readonly string Id;

        public string  Name;
        public Faction Faction;
        public int     Money;
        public int     SurvivalChance;

        public bool IsActive = true;

        public FactionSystemState(string id, string name, Faction faction, int startMoney, int survivalChance) {
            Id             = id;
            Name           = name;
            Faction        = faction;
            Money          = startMoney;
            SurvivalChance = survivalChance;
        }
    }
}
