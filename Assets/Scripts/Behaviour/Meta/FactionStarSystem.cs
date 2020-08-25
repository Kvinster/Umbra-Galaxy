using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.Common;
using STP.State;
using STP.State.Meta;
using STP.Utils.PropertyAttribute;

namespace STP.Behaviour.Meta {
    public sealed class FactionStarSystem : BaseStarSystem {
        static readonly Dictionary<Faction, Color> FactionToColor = new Dictionary<Faction, Color> {
            { Faction.A, Color.green },
            { Faction.B, Color.yellow },
            { Faction.C, Color.cyan }
        };

        [FactionStarSystemId]
        public string         IdText;
        public SpriteRenderer SpriteRenderer;

        ProgressController    _progressController;
        StarSystemsController _starSystemsController;

        public override string Id => IdText;

        public override StarSystemType Type => StarSystemType.Faction;

        public override bool InterruptOnPlayerArriveIntermediate => !_starSystemsController.GetFactionSystemActive(Id);

        new void OnValidate() {
            base.OnValidate();
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
            _starSystemsController.OnStarSystemActiveChanged -= OnStarSystemActiveChanged;
        }

        public override void OnPlayerArrive(bool success) {
            if ( !_progressController.IsActive ) {
                return;
            }
            if ( success && !_starSystemsController.GetFactionSystemActive(Id) ) {
                SceneManager.LoadScene("CoreLevel_DefendSystem");
            }
        }

        protected override void InitSpecific(MetaStarter starter) {
            _progressController    = starter.ProgressController;
            _starSystemsController = starter.StarSystemsController;

            _starSystemsController.OnStarSystemActiveChanged += OnStarSystemActiveChanged;
            OnStarSystemActiveChanged(Id, _starSystemsController.GetFactionSystemActive(Id));
        }

        void OnStarSystemActiveChanged(string starSystemId, bool isActive) {
            if ( starSystemId != Id ) {
                return;
            }
            SpriteRenderer.color = isActive
                ? FactionToColor[_starSystemsController.GetFactionSystemFaction(Id)]
                : Color.gray;
        }
    }
}
