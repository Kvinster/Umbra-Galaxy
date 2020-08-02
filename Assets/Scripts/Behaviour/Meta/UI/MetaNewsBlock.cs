using UnityEngine;
using UnityEngine.UI;

using STP.State.Meta;

namespace STP.Behaviour.Meta.UI {
    public sealed class MetaNewsBlock : MonoBehaviour {
        const string StarSystemCapturedTemplate = "System {0} has been captured by the Darkness";
        const string StarSystemRepelTemplate    = "System {0} successfully repelled attack of the Darkness";

        public GameObject FoldedRoot;
        public GameObject UnfoldedRoot;
        public Button     FoldButton;
        public Button     UnfoldButton;
        [Space]
        public Transform  HeadlinesRoot;
        public GameObject HeadlinePrefab;
        public ScrollRect HeadlinesScrollRect;

        MetaTimeManager       _timeManager;
        StarSystemsController _starSystemsController;
        DarknessController    _darknessController;

        void OnDestroy() {
            _darknessController.OnStarSystemAttack -= OnStarSystemAttack;
        }

        public void Init(MetaTimeManager timeManager) {
            _timeManager = timeManager;
            
            _starSystemsController = StarSystemsController.Instance;
            _darknessController    = DarknessController.Instance;
            _darknessController.OnStarSystemAttack += OnStarSystemAttack;

            FoldButton.onClick.AddListener(() => SetFolded(true));
            UnfoldButton.onClick.AddListener(() => SetFolded(false));

            SetFolded(false);
        }

        void SetFolded(bool isFolded) {
            FoldedRoot.SetActive(isFolded);
            UnfoldedRoot.SetActive(!isFolded);
        }

        void OnStarSystemAttack(string starSystemId, bool starSystemCaptured) {
            var headline = CreateHeadline();
            var systemName = _starSystemsController.GetStarSystemName(starSystemId);
            headline.SetNewsText(_timeManager.CurDay + 1,
                string.Format(starSystemCaptured ? StarSystemCapturedTemplate : StarSystemRepelTemplate, systemName));
            HeadlinesScrollRect.StopMovement();
            LayoutRebuilder.ForceRebuildLayoutImmediate(HeadlinesScrollRect.transform as RectTransform);
            HeadlinesScrollRect.verticalNormalizedPosition = 0f;
        }

        MetaNewsHeadline CreateHeadline() {
            var headlineGo = Instantiate(HeadlinePrefab, HeadlinesRoot);
            var headline = headlineGo.GetComponent<MetaNewsHeadline>();
            if ( !headline ) {
                Debug.LogError("No MetaNewsHeadline component on HeadlinePrefab");
            }
            return headline;
        }
    }
}
