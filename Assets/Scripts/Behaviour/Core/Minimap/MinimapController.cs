using UnityEngine;
using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.Behaviour.Utils;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Minimap {
	public sealed class MinimapController : BaseCoreComponent {
		[NotNull] public PressButton ZoomInButton;
		[NotNull] public PressButton ZoomOutButton;
		[NotNull] public Button      ResetButton;

		MinimapManager _minimapManager;

		void Update() {
			if ( Input.GetKey(KeyCode.KeypadPlus) ) {
				ZoomIn();
			} else if ( Input.GetKey(KeyCode.KeypadMinus) ) {
				ZoomOut();
			} else if ( Input.GetKeyDown(KeyCode.Keypad0) ) {
				ResetZoom();
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			_minimapManager = starter.MinimapManager;

			ZoomInButton.OnPressed.AddListener(ZoomIn);
			ZoomOutButton.OnPressed.AddListener(ZoomOut);
			ResetButton.onClick.AddListener(ResetZoom);
		}

		void ZoomIn() {
			_minimapManager.ZoomInRaw();
		}

		void ZoomOut() {
			_minimapManager.ZoomOutRaw();
		}

		void ResetZoom() {
			_minimapManager.ResetZoom();
		}
	}
}
