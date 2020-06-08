using UnityEngine;

using System.Collections.Generic;

using STP.State;

namespace STP.Gameplay {
    public class MaterialCreator {
        const string PrefabsPath = "Prefabs/CoreMaterials/";
        
        CoreStarter _starter;
        Transform   _root;
        
        Dictionary<string, GameObject> _materialPrefabs = new Dictionary<string, GameObject>();
        
        public MaterialCreator(CoreStarter starter) {
            _starter = starter;
            var materials = Resources.LoadAll<GameObject>(PrefabsPath);
            foreach ( var material in materials ) {
                _materialPrefabs.Add(material.name, material);
            }
            _root = starter.MaterialSpawnStock;
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
            var go = GameObject.Instantiate(_materialPrefabs[itemName], _root);
            go.transform.position = position;
            go.GetComponent<CollectableItem>().Init(_starter);
            return go;
        }
    }
}