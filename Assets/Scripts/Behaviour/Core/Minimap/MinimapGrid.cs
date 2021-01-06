﻿using UnityEngine;
using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Minimap {
	public sealed class MinimapGrid : BaseCoreComponent {
		[NotNull] public RawImage RawImage;

		float   _speed;
		Vector2 _baseSize;
		Vector2 _size;
		Vector2 _offset;

		Transform      _playerTransform;
		MinimapManager _minimapManager;

		void Update() {
			RawImage.uvRect = new Rect((Vector2) _playerTransform.position * _speed + _offset, _size);
		}

		protected override void InitInternal(CoreStarter starter) {
			_playerTransform = starter.Player.transform;
			_minimapManager  = starter.MinimapManager;

			_speed    = 4f / _minimapManager.CameraSize;
			_baseSize = RawImage.uvRect.size;

			_minimapManager.OnCurZoomChanged += OnMinimapZoomChanged;
			OnMinimapZoomChanged(_minimapManager.CurZoom);
		}

		void OnMinimapZoomChanged(float zoom) {
			_size = _baseSize * zoom;
			_offset = (_baseSize - _size) / 2f;
		}
	}
}
