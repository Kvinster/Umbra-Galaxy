using STP.Behaviour.Meta;

namespace STP.Behaviour.Starter {
    public sealed class MetaStarter : BaseStarter<MetaStarter> {
        public PlayerShip           PlayerShip;
        public MetaTimeManager      TimeManager;
        public StarSystemsGraphInfo StarSystemsGraphInfo;
        
        void Start() {
            InitComponents();
        }
    }
}
