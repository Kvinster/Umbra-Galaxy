using UnityEngine;
using UnityEngine.SceneManagement;

using STP.Behaviour.Starter;
using STP.Common;
using STP.State;
using STP.State.Meta;
using STP.Utils.PropertyAttribute;

namespace STP.Behaviour.Meta {
    public sealed class ShardStarSystem : BaseStarSystem {
        [ShardStarSystemId]
        public string IdText;

        public SpriteRenderer SpriteRenderer;

        ProgressController    _progressController;
        StarSystemsController _starSystemsController;

        public override string Id => IdText;

        public override StarSystemType Type => StarSystemType.Shard;

        public override bool InterruptOnPlayerArriveIntermediate => true;

        new void OnValidate() {
            base.OnValidate();
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
            _starSystemsController.OnStarSystemActiveChanged -= OnStarSystemActiveChanged;
        }

        public override void OnPlayerArrive(bool success) {
            if ( !_progressController.IsActive ) {
                return;
            }
            if ( success ) {
                _starSystemsController.SetShardSystemActive(Id, false);
                SceneManager.LoadScene("CoreLevel1");
            }
        }

        protected override void InitSpecific(MetaStarter starter) {
            _progressController    = starter.ProgressController;
            _starSystemsController = starter.StarSystemsController;
            _starSystemsController.OnStarSystemActiveChanged += OnStarSystemActiveChanged;
            OnStarSystemActiveChanged(Id, _starSystemsController.GetShardSystemActive(Id));
        }

        protected override void OnClick() {
            if ( _starSystemsController.GetShardSystemActive(Id) ) {
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
