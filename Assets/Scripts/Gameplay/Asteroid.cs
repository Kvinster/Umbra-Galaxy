using UnityEngine;

using STP.State;
using STP.View;

namespace STP.Gameplay {
    public class Asteroid : CoreBehaviour{
        MaterialCreator    _materialCreator;
        
        public override void Init(CoreStarter starter) {
            _materialCreator = starter.MaterialCreator;
        }
        
        protected override void CheckDescription() { }
        
        void OnDestroy() {
            var random = Random.Range(0, ItemNames.AllItems.Length);
            _materialCreator.CreateMaterial(ItemNames.AllItems[random], transform.parent);
        }
    }
}