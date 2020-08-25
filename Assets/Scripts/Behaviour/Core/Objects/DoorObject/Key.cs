using STP.Behaviour.Starter;
using STP.State;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Objects.DoorObject {
    public class Key : CoreComponent, ICollectable {
        [NotNullOrEmpty] public string Value;

        CorePlayerController _corePlayerController;

        public override void Init(CoreStarter starter) {
            _corePlayerController = starter.CorePlayerController;
        }

        public void CollectItem() {
            _corePlayerController.AddKey(Value);
            Destroy(gameObject);
        }
    }
}