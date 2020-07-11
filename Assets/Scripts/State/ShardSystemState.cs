namespace STP.State {
    public sealed class ShardSystemState {
        public readonly string Id;

        public bool IsActive;

        public ShardSystemState(string id, bool isActive) {
            Id       = id;
            IsActive = isActive;
        }
    }
}
