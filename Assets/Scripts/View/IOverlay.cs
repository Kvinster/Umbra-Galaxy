namespace STP.View {
    public interface IOverlay {
        bool Active { get; }
        void Deinit();
    }
}