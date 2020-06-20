namespace STP.Gameplay {
    public readonly struct ShipInfo {
        public readonly int   Hp;
        public readonly float MaxSpeed;

        public ShipInfo(int hp, float maxSpeed) {
            Hp       = hp;
            MaxSpeed = maxSpeed;
        }
    }
}