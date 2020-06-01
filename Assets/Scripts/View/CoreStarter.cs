using STP.Utils;

namespace STP.View {
    public class CoreStarter : GameComponent
    {
        void Start() {
            foreach (var comp in CoreComponent.Instances) {
                comp.Init(this);
            }        
        }

        protected override void CheckDescription() {
            throw new System.NotImplementedException();
        }
    }
}
