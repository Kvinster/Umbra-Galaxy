using UnityEngine;
using UnityEngine.UI;

using STP.State.Meta;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Meta.UI {
    public sealed class MetaNewsBlock : GameBehaviour {
        const string StarSystemCapturedTemplate = "System {0} has been captured by the Darkness";
        const string StarSystemRepelTemplate    = "System {0} successfully repelled attack of the Darkness";

        [NotNull] public GameObject FoldedRoot;
        [NotNull] public GameObject UnfoldedRoot;
        [NotNull] public Button     FoldButton;
        [NotNull] public Button     UnfoldButton;
        [Space]
        [NotNull] public Transform  HeadlinesRoot;
        [NotNull] public GameObject HeadlinePrefab;
        [NotNull] public ScrollRect HeadlinesScrollRect;

        MetaTimeManager       _timeManager;
        StarSystemsController _starSystemsController;
        DarknessController    _darknessController;

        void OnDestroy() {
            _darknessController.OnStarSystemAttack -= OnStarSystemAttack;
        }

        public void Init(MetaTimeManager timeManager, StarSystemsController starSystemsController,
            DarknessController darknessController) {
            _timeManager = timeManager;
            
            _starSystemsController = starSystemsController;
            _darknessController    = darknessController;
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
