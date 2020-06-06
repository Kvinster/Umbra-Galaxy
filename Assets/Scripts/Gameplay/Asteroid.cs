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
            var random = Random.Range(0, ItemNames.AllItems.Length);
            _materialCreator.CreateMaterial(ItemNames.AllItems[random], transform.position);
            Destroy(gameObject);
        }
    }
}