using UnityEngine;

using System.Collections.Generic;

namespace STP.Behaviour.Meta {
    [CreateAssetMenu(fileName = "DarknessInfo", menuName = "Create DarknessInfoHolder")]
    public sealed class DarknessInfoHolder : ScriptableObject {
        public const string FullAssetPath = "Assets/Resources/Meta/DarknessInfo.asset";
        public const string ResourcesPath = "Meta/DarknessInfo";
        
        public List<DarknessPath> DarknessPaths = new List<DarknessPath>();
    }
}
