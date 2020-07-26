using UnityEngine;
using UnityEngine.SceneManagement;

using STP.Behaviour.Starter;
using STP.Common;
using STP.State.Meta;
using STP.Utils.PropertyAttribute;

namespace STP.Behaviour.Meta {
    public sealed class ShardStarSystem : BaseStarSystem {
        [ShardStarSystemId]
        public string IdText;

        public SpriteRenderer SpriteRenderer;

        public override string Id => IdText;

        public override StarSystemType Type => StarSystemType.Shard;

        public override bool InterruptOnPlayerArriveIntermediate => true;

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

        void OnDestroy() {
            StarSystemsController.Instance.OnStarSystemActiveChanged -= OnStarSystemActiveChanged;
        }

        public override void OnPlayerArrive(bool success) {
            if ( success ) {
                StarSystemsController.Instance.SetShardSystemActive(Id, false);
                SceneManager.LoadScene("CoreLevel1");
            }
        }

        protected override void InitSpecific(MetaStarter starter) {
            var ssc = StarSystemsController.Instance;
            ssc.OnStarSystemActiveChanged += OnStarSystemActiveChanged;
            OnStarSystemActiveChanged(Id, ssc.GetShardSystemActive(Id));
        }

        protected override void OnClick() {
            if ( StarSystemsController.Instance.GetShardSystemActive(Id) ) {
                base.OnClick();
            }
        }

        void OnStarSystemActiveChanged(string starSystemId, bool isActive) {
            if ( starSystemId != Id ) {
                return;
            }
            gameObject.SetActive(isActive);
            SpriteRenderer.enabled = isActive;
        }
    }
}
