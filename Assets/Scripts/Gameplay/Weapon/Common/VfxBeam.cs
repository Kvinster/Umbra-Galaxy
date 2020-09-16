using UnityEngine;

using STP.Utils.GameComponentAttributes;

namespace STP.Gameplay.Weapon.Common {
    public sealed class VfxBeam : BaseBeam {
        [NotNull] public LineRenderer LineRenderer;
        [NotNull] public Transform    HitParticleSystemTrans;

        readonly Vector3[] _positions = { Vector3.zero, Vector3.zero };

        public float Alpha {
            set {
                var alpha    = Mathf.Clamp01(value);
                var gradient = LineRenderer.colorGradient;
                gradient.SetKeys(gradient.colorKeys, new [] { new GradientAlphaKey(alpha, 0f) });
                LineRenderer.colorGradient = gradient;
            }
        }

        void Start() {
            LineRenderer.useWorldSpace = false;
        }

        public override void SetLength(float length) {
            length += 30f;
            _positions[0] = Vector3.zero;
            var endPos = Vector2.up * length;
            _positions[1] = endPos;
            HitParticleSystemTrans.localPosition = endPos;
            LineRenderer.SetPositions(_positions);
        }
    }
}
