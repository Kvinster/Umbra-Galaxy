using UnityEngine;

namespace STP.Utils {
    public abstract class GameComponent : MonoBehaviour {
        protected virtual void CheckDescription() { }

        protected virtual void Awake() {
            if ( !Application.isPlaying ) {
                return;
            }
            GameComponentUtils.CheckAttributes(this);
            CheckDescription();
        }

        protected void OnValidate() {
            if ( (gameObject.hideFlags & HideFlags.DontSaveInEditor) != 0 ) {
                return;
            }
            GameComponentUtils.CheckAttributes(this);
            CheckDescription();
        }
    }
}