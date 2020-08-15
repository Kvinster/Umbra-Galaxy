using UnityEngine.UI;

using STP.Gameplay;
using STP.Utils;

namespace STP.Behaviour.Core {
    public class GameoverOverlay : GameBehaviour, IOverlay {
        public Button ReturnToMeta;
        
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, ReturnToMeta);

        public bool Active => gameObject.activeInHierarchy;
        
        public void Init(CoreManager coreManager) {
            ReturnToMeta.onClick.RemoveAllListeners();
            ReturnToMeta.onClick.AddListener(coreManager.GoToMeta);
            gameObject.SetActive(true);
        }

        public void Deinit() { 
            gameObject.SetActive(false);
        }
    }
}