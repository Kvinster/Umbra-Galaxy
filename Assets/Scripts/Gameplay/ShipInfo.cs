namespace STP.Gameplay {
    public readonly struct ShipInfo {
        public readonly float Hp;
        public readonly float MaxSpeed;

        public ShipInfo(float hp, float maxSpeed) {
            Hp       = hp;
            MaxSpeed = maxSpeed;
        }
    }
}