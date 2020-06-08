using STP.Behaviour.Meta.UI;
using STP.Behaviour.Starter;

namespace STP.Behaviour.Meta {
    public sealed class FactionStarSystem : BaseStarSystem {
        MetaUiCanvas _metaUiCanvas;
        
        protected override void InitSpecific(MetaStarter starter) {
            _metaUiCanvas = starter.MetaUiCanvas;
        }

        protected override void OnPlayerArrive() {
            _metaUiCanvas.OnPlayerArriveToSystem(Name);
        }
    }
}
