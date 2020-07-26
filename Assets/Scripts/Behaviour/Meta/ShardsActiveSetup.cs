using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Utils.PropertyAttribute;

namespace STP.Behaviour.Meta {
    [CreateAssetMenu(menuName = "Create ShardsActiveSetup", fileName = "ShardsActiveSetup")]
    public sealed class ShardsActiveSetup : ScriptableObject {
        [Serializable]
        public sealed class ShardActiveSetup {
            [ShardStarSystemId]
            public string ShardId;
            public int    ActivationDay;
            public int    ActivationPeriod;
        }
        
        public const string FullAssetPath = "Assets/Resources/Meta/ShardsActiveSetup.asset";
        public const string ResourcesPath = "Meta/ShardsActiveSetup";
        

        public List<ShardActiveSetup> ShardActiveSetups = new List<ShardActiveSetup>();
    }
}
