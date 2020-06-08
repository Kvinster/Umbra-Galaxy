using STP.Common;

namespace STP.State {
    public sealed class StarSystemState {
        public readonly string Name;

        public Faction Faction;
        public int     Money;

        public StarSystemState(string name, Faction faction, int startMoney) {
            Name    = name;
            Faction = faction;
            Money   = startMoney;
        }
    }
}
