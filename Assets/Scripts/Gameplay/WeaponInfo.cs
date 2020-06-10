namespace STP.Gameplay {
    public readonly struct WeaponInfo {
        public readonly string BulletType;
        public readonly float  FirePeriod;
        public readonly float  BulletSpeed;

        public WeaponInfo(string bulletType, float firePeriod, float bulletSpeed) {
            BulletType  = bulletType;
            FirePeriod  = firePeriod;
            BulletSpeed = bulletSpeed;
        }
    }
}