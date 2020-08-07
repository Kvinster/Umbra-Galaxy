﻿using UnityEngine;

using System.Linq;

using STP.Behaviour.Starter;
using STP.State.Meta;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.Meta {
    public sealed class PlayerShipPathView : BaseMetaComponent {
        [NotNull] public PlayerShipMovementController PlayerShipMovementController;
        [NotNull] public GameObject                   DistanceRoot;
        [NotNull] public Transform                    DistanceTrans;
        [NotNull] public TMP_Text                     DistanceText;

        StarSystemsManager    _starSystemsManager;
        StarSystemsController _starSystemsController;

        LineRenderer _lineRenderer;

        protected override void InitInternal(MetaStarter starter) {
            _starSystemsManager    = starter.StarSystemsManager;
            _starSystemsController = starter.StarSystemsController;
            
            _lineRenderer = gameObject.AddComponent<LineRenderer>();

            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _lineRenderer.startWidth = _lineRenderer.endWidth = 2f;
            _lineRenderer.startColor = _lineRenderer.endColor = Color.yellow;

            PlayerShipMovementController.OnDestSystemChanged += OnDestSystemChanged;
            PlayerShipMovementController.OnCurStateChanged   += OnCurStateChanged;

            SetEnabled(false);
        }

        void OnCurStateChanged(PlayerShipMovementController.State newState) {
            SetEnabled(newState == PlayerShipMovementController.State.Selected);
        }

        void OnDestSystemChanged(BaseStarSystem newDestSystem) {
            if ( newDestSystem ) {
                var path = _starSystemsController.GetPath(PlayerShipMovementController.CurSystem.Id,
                    newDestSystem.Id);
                _lineRenderer.positionCount = path.Path.Count;
                _lineRenderer.SetPositions(path.Path
                    .Select(x => _starSystemsManager.GetStarSystem(x).transform.position).ToArray());
                DistanceTrans.position =
                    _starSystemsManager.GetStarSystem(path.Path[path.Path.Count - 1]).transform.position +
                    new Vector3(30, -30);
                DistanceText.text = path.PathLength.ToString();
            } else {
                _lineRenderer.positionCount = 0;
            }
        }

        void SetEnabled(bool isEnabled) {
            _lineRenderer.enabled = isEnabled;
            DistanceRoot.SetActive(isEnabled);
        }
    }
}
