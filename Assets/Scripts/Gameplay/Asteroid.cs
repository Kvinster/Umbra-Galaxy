using UnityEngine;

using System.Collections.Generic;

using STP.View;

namespace STP.Gameplay {
    public class Asteroid : CoreBehaviour, IDestructable {
        const int ShowAsteroidProbability = 5;
        
        public bool         IsRare;
        public HpBar        HpBar;
        public List<string> MaterialsOverrides;

        float           _curHp;
        MaterialCreator _materialCreator;
        
        float TotalHp => (IsRare) ? 5 : 1;
        
        protected override void CheckDescription() { }
        
        public override void Init(CoreStarter starter) {
            _materialCreator = starter.MaterialCreator;
            _curHp           = TotalHp;
            if ( HpBar ) {
                HpBar.Init();
            }

            if ( IsRare ) {
                var random = Random.Range(0, 100);
                if ( random > ShowAsteroidProbability ) {
                    Destroy(gameObject);
                }
            }
        }
        
        public void GetDamage(float damageAmount = 1) {
            _curHp -= damageAmount;
            if ( HpBar ) {
                HpBar.UpdateBar(_curHp / TotalHp);
            }
            if ( _curHp <= 0f ) {
                _materialCreator.CreateRandomMaterial(transform.position, MaterialsOverrides);
                Destroy(gameObject);
            }
        }
    }
}