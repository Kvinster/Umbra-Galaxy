using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.VFX;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

using Shapes;

namespace STP.Behaviour.Core.Enemy {
	[ExecuteInEditMode]
	public sealed class ConnectorLine : GameComponent {
		const string StartId              = "Start";
		const string EndId                = "End";
		const string FillId               = "Fill";
		const string FillInvertId         = "FillInvert";
		const string CollisionPosId       = "CollisionPos";
		const string CollisionDirectionId = "CollisionDirection";
		const string CollisionEventName   = "OnCollision";

		[NotNull(false)] public Connector Connector;

		BoxCollider2D _collider;
		Line          _line;
		VisualEffect  _visualEffect;

		bool _invert;

		Vector2 _start;
		Vector2 _end;
		float   _thickness;

		bool _isHorizontal;

		void Update() {
#if UNITY_EDITOR
			if ( !Application.isPlaying ) {
				var line         = GetComponentInChildren<Line>();
				var visualEffect = GetComponentInChildren<VisualEffect>();
				if ( !line || !visualEffect ) {
					return;
				}

				var isHorizontal = Mathf.Approximately(line.Start.y, line.End.y);
				if ( visualEffect ) {
					var halfThickness = line.Thickness / 2f;
					var start = line.Start + (isHorizontal ? new Vector3(0, -halfThickness) : new Vector3(-halfThickness, 0));
					var end = line.End + (isHorizontal ? new Vector3(0, halfThickness) : new Vector3(halfThickness, 0));
					var visualEffectStart = visualEffect.GetVector3(StartId);
					var visualEffectEnd   = visualEffect.GetVector3(EndId);
					if ( (visualEffectStart != start) || (visualEffectEnd != end) ) {
						visualEffect.SetVector3(StartId, start);
						visualEffect.SetVector3(EndId, end);
						UnityEditor.EditorUtility.SetDirty(visualEffect);
					}
				}
			}
#endif
		}

		void Start() {
			if ( !Application.isPlaying ) {
				return;
			}
			_collider     = GetComponentInChildren<BoxCollider2D>();
			_line         = GetComponentInChildren<Line>();
			_visualEffect = GetComponentInChildren<VisualEffect>();
			Assert.IsTrue(_collider);
			Assert.IsTrue(_line);
			Assert.IsTrue(_visualEffect);

			_start     = _line.Start;
			_end       = _line.End;
			_thickness = _line.Thickness;

			_isHorizontal = Mathf.Approximately(_line.Start.y, _line.End.y);

			var halfThickness = _line.Thickness / 2f;
			var start = _line.Start + (_isHorizontal ? new Vector3(0, -halfThickness) : new Vector3(-halfThickness, 0));
			var end   = _line.End + (_isHorizontal ? new Vector3(0, halfThickness) : new Vector3(halfThickness, 0));
			_visualEffect.SetVector3(StartId, start);
			_visualEffect.SetVector3(EndId, end);
			_visualEffect.SetFloat(FillId, 1f);
			if ( Connector ) {
				Connector.OnHpChanged    += OnConnectorHpChanged;
				Connector.OnStartedDying += OnConnectorStartedDying;
				OnConnectorHpChanged(Connector.Hp);
				SetupCollider(Connector.Hp);
			}
		}

		void OnDestroy() {
			if ( Connector ) {
				Connector.OnHpChanged    -= OnConnectorHpChanged;
				Connector.OnStartedDying -= OnConnectorStartedDying;
			}
		}

		void SetupCollider(float hp) {
			Vector2 diff;
			if ( Mathf.Approximately(hp, 1f) ) {
				diff             = _end - _start;
				_collider.offset = diff / 2;
			} else {
				if ( _invert ) {
					var end = Vector2.Lerp(_end, _start, 1f - Mathf.Clamp01(hp));
					diff             = end - _start;
					_collider.offset = diff / 2;
				} else {
					var start = Vector2.Lerp(_start, _end, 1f - Mathf.Clamp01(hp));
					diff = _end - start;
					_collider.offset = start + diff / 2;
				}
			}
			var size = new Vector2(
				Mathf.Abs(!Mathf.Approximately(diff.x, 0) ? diff.x : 0) + _thickness,
				Mathf.Abs(!Mathf.Approximately(diff.y, 0) ? diff.y : 0) + _thickness);
			_collider.size = size;
		}

		void OnConnectorHpChanged(float hp) {
			_visualEffect.SetFloat(FillId, Mathf.Clamp01(hp));
			SetupCollider(hp);
		}

		void OnConnectorStartedDying(bool fromMainConnector) {
			_invert = fromMainConnector;
			_visualEffect.SetBool(FillInvertId, fromMainConnector);
			SetupCollider(Connector.Hp);
		}

		void OnCollisionEnter2D(Collision2D other) {
			if ( !Application.isPlaying ) {
				return;
			}
			if ( other.gameObject.GetComponent<Bullet>() ) {
				var worldContact = other.contacts[0].point;
				_visualEffect.SetVector2(CollisionPosId, _visualEffect.transform.InverseTransformPoint(worldContact));
				Vector2 direction;
				if ( _isHorizontal ) {
					direction = (worldContact.y > transform.position.y) ? Vector2.up : Vector2.down;
				} else {
					direction = (worldContact.x > transform.position.x) ? Vector2.right : Vector2.left;
				}
				_visualEffect.SetVector2(CollisionDirectionId, direction);
				_visualEffect.SendEvent(CollisionEventName);
			}
		}
	}
}
