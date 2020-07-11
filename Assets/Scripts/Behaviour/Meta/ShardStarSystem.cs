using UnityEngine;
using UnityEngine.SceneManagement;

using STP.Behaviour.Starter;
using STP.Common;
using STP.Utils.PropertyAttribute;

namespace STP.Behaviour.Meta {
    public sealed class ShardStarSystem : BaseStarSystem {
        [ShardStarSystemId]
        public string IdText;

        public override string Id => IdText;

        public override StarSystemType Type => StarSystemType.Shard;
        
        void OnValidate() {
#if UNITY_EDITOR
            if ( !UnityEditor.PrefabUtility.IsPartOfAnyPrefab(this) ||
                 UnityEditor.PrefabUtility.IsPartOfPrefabInstance(this) ) {
                var graphInfo =
                    Resources.Load<StarSystemsGraphInfoScriptableObject>(StarSystemsGraphInfoScriptableObject
                        .ResourcesPath);
                if ( !graphInfo ) {
                    return;
                }
                var starSystemName = graphInfo.StarSystemsGraphInfo.GetShardSystemName(Id);
                gameObject.name = starSystemName;
                if ( StarSystemNameText && !string.IsNullOrEmpty(gameObject.scene.name) &&
                     (StarSystemNameText.text != starSystemName) ) {
                    StarSystemNameText.text = starSystemName;
                    UnityEditor.EditorUtility.SetDirty(StarSystemNameText);
                }
            }
#endif
        }

        protected override void InitSpecific(MetaStarter starter) {
        }

        protected override void OnPlayerArrive(bool success) {
            if ( success ) {
                SceneManager.LoadScene("CoreLevel1");
            }
        }
    }
}
