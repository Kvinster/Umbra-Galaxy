using UnityEngine;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public sealed class DamageVignette : GameComponent {
		static readonly int Color    = Shader.PropertyToID("_Color");
		static readonly int Progress = Shader.PropertyToID("_Progress");
		static readonly int Center   = Shader.PropertyToID("_Center");
		static readonly int SmoothId = Shader.PropertyToID("_Smooth");

		[NotNull]
		public SpriteRenderer SpriteRenderer;
		[Range(0f, 1f)]
		public float Smooth;

		MaterialPropertyBlock _mpb;
		MaterialPropertyBlock MaterialPropertyBlock {
			get {
				if ( _mpb == null ) {
					_mpb = new MaterialPropertyBlock();
					SpriteRenderer.GetPropertyBlock(_mpb);
				}
				return _mpb;
			}
		}

		Color _color;

		void Start() {
			_color = SpriteRenderer.color;

			MaterialPropertyBlock.SetColor(Color, GetColor(0f));
			MaterialPropertyBlock.SetVector(Center, Vector3.zero);
			MaterialPropertyBlock.SetFloat(SmoothId, Smooth);
			MaterialPropertyBlock.SetFloat(Progress, 0f);
			SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
		}

		public void SetEffectValue(float progress) {
			progress = Mathf.Clamp01(progress);
			MaterialPropertyBlock.SetColor(Color, GetColor(progress));
			MaterialPropertyBlock.SetFloat(Progress, progress);
			SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
		}

		Color GetColor(float alphaMult) {
			return new Color(_color.r, _color.g, _color.b, _color.a * alphaMult);
		}
	}
}
