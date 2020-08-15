using UnityEngine;

using STP.Config;

namespace STP.State {
    public sealed class DialogController : BaseStateController {
        DialogConfig _config;

        public override void Init() {
            _config = ConfigLoader.LoadConfig<DialogConfig>();
        }

        public DialogInfo GetDialogInfo(string dialogName) {
            foreach ( var dialogInfo in _config.Dialogs ) {
                if ( dialogInfo.DialogName == dialogName ) {
                    return dialogInfo;
                }
            }
            Debug.LogErrorFormat("No DialogInfo for name '{0}'", dialogName);
            return null;
        }
    }
}
