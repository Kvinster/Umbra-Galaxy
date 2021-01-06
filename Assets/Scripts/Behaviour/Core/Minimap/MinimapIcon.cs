using UnityEngine;

using STP.Behaviour.Starter;
using STP.Manager;

using Shapes;

namespace STP.Behaviour.Core.Minimap {
	public class MinimapIcon : BaseCoreComponent {
		Vector3 _baseScale;

		MinimapManager _minimapManager;

		protected override void CheckDescription() {
			if ( LayerMask.LayerToName(gameObject.layer) != "Minimap" ) {
				Debug.LogError("Invalid minimap icon game object layer", this);
			}
		}

		void OnDestroy() {
			if ( _minimapManager != null ) {
				_minimapManager.OnCurZoomChanged -= OnMinimapZoomChanged;
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			_baseScale                       =  transform.localScale;
			_minimapManager                  =  starter.MinimapManager;
			_minimapManager.OnCurZoomChanged += OnMinimapZoomChanged;
			OnMinimapZoomChanged(_minimapManager.CurZoom);

			var sr = GetComponent<ShapeRenderer>();
			if ( sr ) {
				sr.enabled = true;
			}
		}

		protected virtual void OnMinimapZoomChanged(float zoom) {
			transform.localScale = _baseScale * zoom;
		}
	}
}
