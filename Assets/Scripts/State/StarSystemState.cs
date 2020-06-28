using STP.Common;

namespace STP.State {
    public sealed class StarSystemState {
        public readonly string Name;
        
        public Faction Faction;
        public int     Money;
        public int     SurvivalChance;

        public bool IsActive = true;

        public StarSystemState(string name, Faction faction, int startMoney, int survivalChance) {
            Name           = name;
            Faction        = faction;
            Money          = startMoney;
            SurvivalChance = survivalChance;
        }
    }
}
