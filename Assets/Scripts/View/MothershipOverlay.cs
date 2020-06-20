using UnityEngine.UI;

using STP.Gameplay;
using STP.Utils;

namespace STP.View {
    public class MothershipOverlay : GameBehaviour, IOverlay {
        public Button ReturnToMeta;
        public Button DropAndContinue;
        
        CoreManager _coreManager;
        
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, ReturnToMeta, DropAndContinue);

        public bool Active => gameObject.activeInHierarchy;

        public void Init(CoreManager coreManager) {
            _coreManager = coreManager;
            ReturnToMeta.onClick.RemoveAllListeners();
            ReturnToMeta.onClick.AddListener(()=>coreManager.GoToShop(true));
            DropAndContinue.onClick.RemoveAllListeners();
            DropAndContinue.onClick.AddListener(DropItems);
            gameObject.SetActive(true);
        }

        public void Deinit() { 
            gameObject.SetActive(false);
        }

        void DropItems() {
            _coreManager.SendItemsToMothership();
            gameObject.SetActive(false);
        }
    }
}