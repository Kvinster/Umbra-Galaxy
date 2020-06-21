using UnityEngine;

namespace STP.Behaviour.Meta {
    [CreateAssetMenu(menuName = "Create StarSystemsGraphInfoScriptableObject", fileName = "StarSystems")]
    public sealed class StarSystemsGraphInfoScriptableObject : ScriptableObject {
        public const string FullAssetPath = "Assets/Resources/Meta/StarSystems.asset";
        public const string ResourcesPath = "Meta/StarSystems";
        
        public StarSystemsGraphInfo StarSystemsGraphInfo = new StarSystemsGraphInfo();

        public static StarSystemsGraphInfoScriptableObject LoadFromResources() {
            return Resources.Load<StarSystemsGraphInfoScriptableObject>(ResourcesPath);
        }
    }
}
