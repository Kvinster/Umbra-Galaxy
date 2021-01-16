using UnityEngine;
using UnityEngine.VFX;

using System.Collections.Generic;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public class VfxRunner : GameComponent {
		[NotNull] public List<VisualEffect> Effects;

		bool _spawned;
		bool _destroyOnEnd;

		public bool Running { get; private set; }

		void Start() {
			StopVfx();
		}

		void Update() {
			if ( !Running ) {
				return;
			}
			var maxParticlesLeft = 0;
			foreach ( var effect in Effects ) {
				maxParticlesLeft = Mathf.Max(maxParticlesLeft, effect.aliveParticleCount);
			}
			_spawned |= (maxParticlesLeft > 0);
			if ( _spawned && (maxParticlesLeft == 0) ) {
				Running = false;
				if ( _destroyOnEnd ) {
					Destroy(gameObject);
				}
			}
		}

		public void RunVfx(bool destroyOnEnd) {
			foreach ( var effect in Effects ) {
				effect.Play();
			}
			Running       = true;
			_destroyOnEnd = destroyOnEnd;
		}

		public void StopVfx() {
			foreach ( var effect in Effects ) {
				effect.Stop();
			}
		}
	}
}