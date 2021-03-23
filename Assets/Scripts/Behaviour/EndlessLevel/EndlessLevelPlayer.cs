
using UnityEngine;

using STP.Behaviour.Starter;
using STP.Common;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.EndlessLevel {
	public sealed class EndlessLevelPlayer : BaseEndlessLevelComponent {
		[NotNull] 
		public ShootingSystemParams ShootingParams;
		
		public float MovementSpeed;
		[NotNull]
		public Rigidbody2D Rigidbody;

		ShootingSystem _shootingSystem;
		
		Camera _camera;
		
		Vector2 _input;
		
		protected override void InitInternal(EndlessLevelStarter starter) {
			_camera = starter.Camera;
			_shootingSystem = new ShootingSystem(ShootingParams);
		}

		void Update() {
			if ( !IsInit ) {
				return;
			}
			_input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			_shootingSystem.DeltaTick();
			if ( Input.GetMouseButton(0) ) {
				_shootingSystem.TryShoot();
			}
		}

		void FixedUpdate() {
			if ( !IsInit ) {
				return;
			}
			if ( _input != Vector2.zero ) {
				Rigidbody.AddForce(_input.normalized * (Rigidbody.mass * MovementSpeed), ForceMode2D.Impulse);
			}
			var mouseWorldPos  = _camera.ScreenToWorldPoint(Input.mousePosition);
			var neededRotation = -Vector2.SignedAngle(mouseWorldPos - transform.position, Vector2.up);
			Rigidbody.MoveRotation(neededRotation);
			_input = Vector2.zero;
		}
	}
}