using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	[RequireComponent(typeof(RectTransform))]
	public sealed class BossChunk : BaseCoreComponent {
		[NotNull] public Transform PlayerSpawnPosition;

		RectTransform _rectTransform;

		RectTransform CachedRectTransform {
			get {
				if ( !_rectTransform ) {
					_rectTransform = GetComponent<RectTransform>();
				}
				return _rectTransform;
			}
		}

		public Vector2 AreaSize => CachedRectTransform.sizeDelta;

		protected override void InitInternal(CoreStarter starter) {
			var cam        = starter.MainCamera;
			var areaHeight = cam.orthographicSize * 2;
			var areaWidth  = cam.aspect * areaHeight;
			_rectTransform.sizeDelta = new Vector2(areaWidth, areaHeight);
		}
	}
}
