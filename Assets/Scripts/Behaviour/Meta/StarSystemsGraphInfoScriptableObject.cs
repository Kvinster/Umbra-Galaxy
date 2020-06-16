using UnityEngine;

namespace STP.Behaviour.Meta {
    [CreateAssetMenu(menuName = "Create StarSystemsGraphInfoScriptableObject", fileName = "StarSystems")]
    public sealed class StarSystemsGraphInfoScriptableObject : ScriptableObject {
        public StarSystemsGraphInfo StarSystemsGraphInfo = new StarSystemsGraphInfo();
    }
}
