﻿using UnityEngine;

using System.Collections.Generic;

using STP.State;

namespace STP.Gameplay {
    public class MaterialCreator {
        CoreStarter _starter;
        
        const string PrefabsPathFormat = "Prefabs/CoreMaterials/{0}";
        Dictionary<string, GameObject> _materialPrefabs = new Dictionary<string, GameObject>();
        
        public MaterialCreator(CoreStarter starter) {
            _starter = starter;
            foreach ( var material in ItemNames.AllItems ) {
                var path = string.Format(PrefabsPathFormat, material);
                var go   = Resources.Load<GameObject>(path);
                if ( go ) {
                    _materialPrefabs.Add(material, go);
                }
            }
        }

        public GameObject CreateRandomMaterial(Vector3 position) {
            var random = Random.Range(0, ItemNames.AllItems.Length);
            return CreateMaterial(ItemNames.AllItems[random], position);
        }

        public GameObject CreateMaterial(string itemName, Vector3 position) {
            if ( !_materialPrefabs.ContainsKey(itemName) ) {
                Debug.LogError(string.Format("Can't find material {0} in loaded materials", itemName));
                return null;
            }
            var go = GameObject.Instantiate(_materialPrefabs[itemName], position, Quaternion.identity);
            go.GetComponent<CollectableItem>().Init(_starter);
            return go;
        }
    }
}