using UnityEngine.UI;

using System.Collections.Generic;

using STP.Utils.GameComponentAttributes;

using RSG;

namespace STP.Behaviour.Core.UI {
	public class LevelUpWindow : BaseCoreWindow {
		[NotNullOrEmpty] public List<Button> Buttons;
		
		public void CommonInit() {
		}

		public override IPromise Show() {
			foreach ( var button in Buttons ) {
				button.onClick.AddListener(Hide);
			}
			return base.Show();
		}

		protected override void Hide() {
			base.Hide();
			foreach ( var button in Buttons ) {
				button.onClick.RemoveAllListeners();
			}
		}
	}
}