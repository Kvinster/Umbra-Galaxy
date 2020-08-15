namespace STP.Behaviour.Core {
    public interface IOverlay {
        bool Active { get; }
        void Deinit();
    }
}