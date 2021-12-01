using UnityEngine;
using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Minimap {
	public sealed class MinimapGrid : GameComponent {
		[NotNull] public RawImage RawImage;

		float   _speed;
		Vector2 _baseSize;
		Vector2 _size;
		Vector2 _offset;

		Transform      _playerTransform;
		MinimapManager _minimapManager;

		bool _isInit;

		void Update() {
			if ( !_isInit ) {
				return;
			}
			RawImage.uvRect = new Rect((Vector2) _playerTransform.position * _speed + _offset, _size);
		}

		void OnDestroy() {
			if ( _minimapManager != null ) {
				_minimapManager.OnCurZoomChanged -= OnMinimapZoomChanged;
			}
		}

		public void Init(Player player, MinimapManager minimapManager) {
			_playerTransform = player.transform;
			_minimapManager  = minimapManager;

			_speed    = 4f / _minimapManager.CameraSize;
			_baseSize = RawImage.uvRect.size;

			_minimapManager.OnCurZoomChanged += OnMinimapZoomChanged;
			OnMinimapZoomChanged(_minimapManager.CurZoom);

			_isInit = true;
		}

		void OnMinimapZoomChanged(float zoom) {
			_size = _baseSize * zoom;
			_offset = (_baseSize - _size) / 2f;
		}
	}
}
