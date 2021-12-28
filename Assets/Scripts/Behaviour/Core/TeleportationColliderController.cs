using System;
using STP.Utils;
using STP.Utils.GameComponentAttributes;
using UnityEngine;

namespace STP.Behaviour.Core {
	[RequireComponent(typeof(BoxCollider2D))]
	public class TeleportationColliderController : GameComponent {
		public bool KillOnTeleportation;
			
		[NotNull] public BoxCollider2D BoxCollider2D;

		public Rect Rect;

		protected void Reset() {
			BoxCollider2D           = GetComponent<BoxCollider2D>();
			BoxCollider2D.isTrigger = true;
			gameObject.layer        = LayerMask.NameToLayer("TeleportationLayer");
			if ( TryGetRect(gameObject, out var rect) ) {
				Rect                    = rect;
				BoxCollider2D.size      = Rect.size;
			}
		}

		void OnDrawGizmos() {
			var aabb = CalcAABBRect();
			aabb.size *= transform.lossyScale;
			Gizmos.DrawWireCube(aabb.center, aabb.size);
		}

		public Rect CalcAABBRect() {
			var rotationAngle = transform.rotation.eulerAngles.z;
			
			TryGetRect(gameObject, out var sourceRect);
			
			var rotationSin = Mathf.Abs(Mathf.Sin(rotationAngle * Mathf.Deg2Rad));
			var rotationCos = Mathf.Abs(Mathf.Cos(rotationAngle * Mathf.Deg2Rad));
			
			var aabbRectSize = new Vector2(
				sourceRect.height * rotationSin + sourceRect.width * rotationCos,
				sourceRect.height * rotationCos + sourceRect.width * rotationSin);
			
			var res = new Rect(Vector2.zero, aabbRectSize) {
				center = transform.position
			};

			return res;
		}
		
		bool TryGetRect(GameObject obj, out Rect rect) {
			var sr = obj.GetComponentInChildren<SpriteRenderer>();
			if ( sr ) {
				var srRect = sr.sprite.rect;
				srRect.size /= sr.sprite.pixelsPerUnit;
				rect        =  srRect;
				return true;
			}
			rect             = Rect.zero;
			
			return false;
		}
	}
}