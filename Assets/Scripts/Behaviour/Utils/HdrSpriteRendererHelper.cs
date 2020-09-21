using UnityEngine;

using STP.Utils;

namespace STP.Behaviour.Utils {
    [RequireComponent(typeof(SpriteRenderer))]
    [ExecuteInEditMode]
    public sealed class HdrSpriteRendererHelper : GameComponent {
        static readonly int HdrColorId = Shader.PropertyToID("_HdrColor");

        [ColorUsage(true, true)]
        public Color HdrColor = Color.white;

        SpriteRenderer _spriteRenderer;
        Color          _oldColor;

        MaterialPropertyBlock _mpb;

        SpriteRenderer SpriteRenderer => Application.isPlaying ? _spriteRenderer : GetComponent<SpriteRenderer>();

        MaterialPropertyBlock MaterialPropertyBlock {
            get {
                if ( _mpb == null ) {
                    _mpb = new MaterialPropertyBlock();
                    SpriteRenderer.GetPropertyBlock(_mpb);
                }
                return _mpb;
            }
        }

        void OnEnable() {
            if ( !_spriteRenderer ) {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
            _oldColor = HdrColor;
            SetColor(HdrColor);
        }

        void Update() {
            if ( _oldColor != HdrColor ) {
                SetColor(HdrColor);
            }
        }

        public void SetColor(Color color) {
            _oldColor = color;
            MaterialPropertyBlock.SetColor(HdrColorId, _oldColor);
            SpriteRenderer.SetPropertyBlock(_mpb);
        }
    }
}
