using UnityEngine.VFX;

using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public sealed class StarsBackground : BaseCoreComponent {
		const string AreaPosId  = "AreaPos";
		const string AreaSizeId = "AreaSize";

		[NotNull] public VisualEffect VisualEffect;

		protected override void InitInternal(CoreStarter starter) {
			VisualEffect.SetVector2(AreaPosId, starter.AreaRect.center);
			VisualEffect.SetVector2(AreaSizeId, starter.AreaRect.size);
		}
	}
}
