using UnityEngine;

using STP.Behaviour.Starter;
using STP.State;

namespace STP.Behaviour.Meta {
    public sealed class FactionStarSystem : BaseStarSystem {
        public SpriteRenderer SpriteRenderer;
        
        protected override void InitSpecific(MetaStarter starter) {
            StarSystemsController.Instance.OnStarSystemActiveChanged += OnStarSystemActiveChanged;
            OnStarSystemActiveChanged(Name, StarSystemsController.Instance.GetStarSystemActive(Name));
        }

        void OnStarSystemActiveChanged(string starSystemName, bool isActive) {
            if ( starSystemName != Name ) {
                return;
            }
            SpriteRenderer.color = isActive ? Color.white : Color.red;
        }
    }
}
