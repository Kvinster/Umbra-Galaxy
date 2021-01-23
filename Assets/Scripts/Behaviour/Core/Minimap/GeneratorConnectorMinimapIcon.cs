using STP.Behaviour.Core.Enemy;
using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

using Shapes;

namespace STP.Behaviour.Core.Minimap {
	public sealed class GeneratorConnectorMinimapIcon : MinimapIcon {
		[NotNull] public Line      Line;
		[NotNull] public float     ThicknessMult;
		[NotNull] public Connector Connector;

		float _baseThickness;

		void OnDestroy() {
			if ( Connector ) {
				Connector.OnInit -= OnConnectorInit;
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			// base.InitInternal(starter);
			// _baseThickness = Connector.Line.Thickness;
			// if ( Connector.IsInit ) {
			// 	OnConnectorInit();
			// } else {
			// 	Connector.OnInit += OnConnectorInit;
			// }
		}

		protected override void OnMinimapZoomChanged(float zoom) {
			Line.Thickness = _baseThickness * zoom * ThicknessMult;
		}

		void OnConnectorInit() {
			// Connector.OnInit -= OnConnectorInit;
			//
			// Line.Start = Connector.Line.Start;
			// Line.End   = Connector.Line.End;
		}
	}
}
