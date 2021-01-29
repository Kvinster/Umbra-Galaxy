using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

using Shapes;

namespace STP.Behaviour.Core.Minimap {
	public sealed class GeneratorConnectorMinimapIcon : MinimapIcon {
		[NotNull] public Line  Line;
		[NotNull] public float ThicknessMult;

		[NotNull(false)] public Line      SourceLine;

		float _baseThickness;

		protected override void InitInternal(CoreStarter starter) {
			_baseThickness = SourceLine.Thickness;
			Line.Start     = SourceLine.Start;
			Line.End       = SourceLine.End;
			Line.Thickness = _baseThickness;
			base.InitInternal(starter);
		}

		protected override void OnMinimapZoomChanged(float zoom) {
			Line.Thickness = _baseThickness * zoom * ThicknessMult;
		}
	}
}
