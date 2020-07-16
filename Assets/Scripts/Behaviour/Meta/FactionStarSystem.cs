using UnityEngine;
using UnityEngine.SceneManagement;

using STP.Behaviour.Starter;
using STP.Common;
using STP.State.Meta;
using STP.Utils.PropertyAttribute;

namespace STP.Behaviour.Meta {
    public sealed class FactionStarSystem : BaseStarSystem {
        [FactionStarSystemId]
        public string         IdText;
        public SpriteRenderer SpriteRenderer;

        public override string Id => IdText;

        public override StarSystemType Type => StarSystemType.Faction;

        void OnValidate() {
#if UNITY_EDITOR
            if ( UnityEditor.PrefabUtility.IsPartOfPrefabInstance(this) ) {
                var graphInfo =
                    Resources.Load<StarSystemsGraphInfoScriptableObject>(StarSystemsGraphInfoScriptableObject
                        .ResourcesPath);
                if ( !graphInfo ) {
                    return;
                }
                var starSystemName = graphInfo.StarSystemsGraphInfo.GetFactionSystemName(Id);
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

        protected override void InitSpecific(MetaStarter starter) {
            StarSystemsController.Instance.OnStarSystemActiveChanged += OnStarSystemActiveChanged;
            OnStarSystemActiveChanged(Id, StarSystemsController.Instance.GetFactionSystemActive(Id));
        }

        protected override void OnPlayerArrive(bool success) {
            if ( success && !StarSystemsController.Instance.GetFactionSystemActive(Id) ) {
                SceneManager.LoadScene("CoreLevel1");
            }
        }

        void OnStarSystemActiveChanged(string starSystemId, bool isActive) {
            if ( starSystemId != Id ) {
                return;
            }
            SpriteRenderer.color = isActive ? Color.white : Color.gray;
        }
    }
}
