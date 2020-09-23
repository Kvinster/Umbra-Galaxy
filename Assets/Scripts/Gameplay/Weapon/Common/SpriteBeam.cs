using UnityEngine;

namespace STP.Gameplay.Weapon.Common {
    public sealed class SpriteBeam : BaseBeam {
        public SpriteRenderer BeamRenderer;

        public override void SetLength(float length) {
            BeamRenderer.size = new Vector2(BeamRenderer.size.x, length);
        }
    }
}