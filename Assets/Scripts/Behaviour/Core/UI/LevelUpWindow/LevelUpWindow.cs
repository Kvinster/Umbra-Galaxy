using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.Core;
using STP.Utils.GameComponentAttributes;

using RSG;

namespace STP.Behaviour.Core.UI.LevelUpWindow {
	public class LevelUpWindow : BaseCoreWindow {
		[NotNullOrEmpty] public List<ShipView> Views;

		XpController _xpController;
		CoreStarter  _starter;
		
		public void CommonInit(XpController xpController, CoreStarter starter) {
			_xpController = xpController;
			_starter      = starter;
		}

		public override IPromise Show() {
			var shipTypes = _xpController.GetLevelUpInfo(_xpController.Level-1).ShipsToSelect;
			var minCount  = Mathf.Min(Views.Count, shipTypes.Count);
			for ( var i = 0; i < minCount; i++ ) {
				var shipType = shipTypes[i];
				Views[i].Init(_starter, _xpController, shipType, Hide);
			}
			_xpController.UseLevelUp();
			return base.Show();
		}
		
		protected override void Hide() {
			base.Hide();
			foreach ( var view in Views ) {
				view.Deinit();
			}
		}
	}
}