using UnityEngine;

using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace STP.Config.ScriptableObjects {
    [CreateAssetMenu(fileName = "CoreLevelCatalogue", menuName = "Create CoreLevelCatalogue")]
    public class CoreLevelsCatalogue : ScriptableObject {
        [Serializable]
        public class LevelPack {
            public LevelType    LevelType;
            public List<string> Levels;
            
            public int LevelsCount => Levels.Count;
        }
        
        public List<LevelPack> LevelPacks;
        public LevelPack       NotSpecifiedLevels = new LevelPack() { LevelType = LevelType.NotSpecificLevels };
        
        public string GetRandomLevelName(LevelType levelType) {
            foreach ( var pack in LevelPacks ) {
                if ( pack.LevelType == levelType && pack.LevelsCount != 0) {
                    return GetRandomLevelFromList(pack.Levels);
                }
            }
            Debug.Log($"Can't find level for type {levelType}");
            return null;
        }

        public string GetNotSpecifiedRandomLevelName() {
            return GetRandomLevelFromList(NotSpecifiedLevels.Levels);
        }

        string GetRandomLevelFromList(List<string> pack) {
            if ( pack.Count == 0 ) {
                Debug.Log("Can't get level. There is 0 levels.");
                return null;
            }
            var randIndex = Random.Range(0, pack.Count);
            return pack[randIndex];
        }
    }
}