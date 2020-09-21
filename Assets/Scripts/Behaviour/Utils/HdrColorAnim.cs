using UnityEngine;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Utils {
    public sealed class HdrColorAnim : GameComponent {
        [NotNull] public HdrSpriteRendererHelper HdrSpriteRendererHelper;
        [ColorUsage(true, true)]
        public Color BaseColor = Color.white;

        [Space]
        public float StartIntensity = 1f;
        public float MinIntensity   = 1f;
        public float MaxIntensity   = 2f;
        public float Speed          = 1f;

        float _intensity = 2f;
        float _direction = 1f;

        protected override void CheckDescription() {
            if ( (StartIntensity < MinIntensity) || (StartIntensity > MaxIntensity) ) {
                StartIntensity = Mathf.Clamp(StartIntensity, MinIntensity, MaxIntensity);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
            if ( MinIntensity > MaxIntensity ) {
                Debug.LogError("HdrColorAnim: wrong description", this);
            }
        }

        void Start() {
            _intensity = StartIntensity;
        }

        void Reset() {
            HdrSpriteRendererHelper = GetComponentInChildren<HdrSpriteRendererHelper>();
        }

        void Update() {
            _intensity += _direction * Speed * Time.deltaTime;

            var factor = Mathf.Pow(2,_intensity);
            var color  = BaseColor;
            color = new Color(color.r * factor,color.g * factor,color.b * factor, color.a);
            HdrSpriteRendererHelper.SetColor(color);

            if ( _intensity <= MinIntensity ) {
                _direction = 1f;
            } else if ( _intensity >= MaxIntensity ) {
                _direction = -1f;
            }
        }
    }
}
