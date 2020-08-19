﻿using STP.Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace STP.Behaviour.Core.Objects {
    public class Asteroid : CoreComponent, IDestructable {
        const int ShowAsteroidProbability = 5;
        
        public bool         IsRare;
        public HpBar        HpBar;
        public List<string> MaterialsOverrides;

        float           _curHp;
        CoreItemCreator _coreItemCreator;
        
        float TotalHp => (IsRare) ? 5 : 1;
        
        protected override void CheckDescription() { }
        
        public override void Init(CoreStarter starter) {
            _coreItemCreator = starter.CoreItemCreator;
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
                _coreItemCreator.CreateRandomMaterial(transform.position, MaterialsOverrides);
                Destroy(gameObject);
            }
        }
    }
}