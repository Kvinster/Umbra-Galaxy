using Shapes;
using STP.Utils;
using STP.Utils.GameComponentAttributes;
using UnityEngine;

namespace STP.Behaviour.Core {
	[RequireComponent(typeof(BoxCollider2D))]
	public class TeleportationColliderController : GameComponent {
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