using STP.State;
using STP.View;
using Random = UnityEngine.Random;

namespace STP.Gameplay {
    public class Asteroid : CoreBehaviour, IDestructable{
        MaterialCreator    _materialCreator;
        
        protected override void CheckDescription() { }
        
        public override void Init(CoreStarter starter) {
            _materialCreator = starter.MaterialCreator;
        }
        
        public void GetDamage(int damageAmount = 1) {
            _materialCreator.CreateRandomMaterial(transform.position);
            Destroy(gameObject);
        }
    }
}