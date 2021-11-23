	using UnityEngine;
using UnityEngine.VFX;

using System;
using System.Collections.Generic;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public class VfxRunner : GameComponent {
		[NotNull] public List<VisualEffect> Effects;

		public bool StopOnStart = true;

		bool _spawned;
		bool _destroyOnEnd;

		Action _scheduledAction;

		readonly Timer _timer = new Timer();

		public bool Running { get; private set; }

		void Start() {
			if ( StopOnStart ) {
				StopVfx();
			}
			else {
				Running       = true;
				_destroyOnEnd = true;
			}
		}

		void Update() {
			if ( _scheduledAction != null && _timer.DeltaTick()) {
				_scheduledAction();
				_scheduledAction = null;
			}
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

		public void ScheduleVfx(float secDelay, bool destroyOnEnd) {
			_scheduledAction = () => RunVfx(destroyOnEnd);
			_timer.Start(secDelay);
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