using STP.Utils;
using STP.Utils.GameComponentAttributes;
using UnityEngine;
using UnityEngine.VFX;

namespace STP.Behaviour.Core {
	public class RigidbodyParamsToVfxSender : GameComponent {
		[NotNull] public VisualEffect VisualEffect;
		[NotNull] public Transform    TransformSource;

		public string RotationParamName;
		
		int _rbAngleId;
		
		public void Start() {
			_rbAngleId    = Shader.PropertyToID(RotationParamName);
			if ( !TransformSource ) {
				return;
			}
			VisualEffect.SetFloat(_rbAngleId, TransformSource.eulerAngles.z);
		}
		
		public void FixedUpdate() {
			if ( !TransformSource ) {
				return;
			}
			VisualEffect.SetFloat(_rbAngleId, TransformSource.eulerAngles.z);
		}
	}
}