using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Minimap {
	public sealed class MinimapController : BaseCoreComponent {
		[NotNull] public Button ZoomInButton;
		[NotNull] public Button ZoomOutButton;

		MinimapManager _minimapManager;

		protected override void InitInternal(CoreStarter starter) {
			_minimapManager = starter.MinimapManager;

			ZoomInButton.onClick.AddListener(_minimapManager.ZoomIn);
			ZoomOutButton.onClick.AddListener(_minimapManager.ZoomOut);
		}
	}
}
