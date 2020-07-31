using UnityEngine;

using STP.State;
using STP.State.Meta;

namespace STP.Behaviour.Meta {
    public sealed class MetaAiShipView : MonoBehaviour {
        public SpriteRenderer ShipIcon;
        
        MetaAiShipManager  _aiShipManager;
        StarSystemsManager _starSystemsManager;
        MetaTimeManager    _timeManager;
        
        TimeController        _timeController;
        StarSystemsController _starSystemsController;

        BaseStarSystem _curSystem;
        BaseStarSystem _destSystem;

        public string Id => State.Id;
        
        public MetaAiShipState State { get; private set; }

        void OnDestroy() {
            _timeController.OnCurDayChanged                  -= OnCurDayChanged;
            _starSystemsController.OnStarSystemActiveChanged -= OnStarSystemActiveChanged;
        }

        void Update() {
            if ( State.CurMode == MetaAiShipMode.Moving ) {
                if ( _timeManager.IsPaused ) {
                    return;
                }
                var progress = (_timeManager.CurDay - State.CurDay + _timeManager.DayProgress) /
                               (State.DestDay - State.CurDay);
                transform.position =
                    Vector2.Lerp(_curSystem.transform.position, _destSystem.transform.position, progress);
            }
        }

        public void Init(MetaAiShipState state, MetaAiShipManager aiShipManager, StarSystemsManager starSystemsManager, 
            MetaTimeManager timeManager) {
            State = state;
            
            _aiShipManager      = aiShipManager;
            _starSystemsManager = starSystemsManager;
            _timeManager        = timeManager;

            _timeController = TimeController.Instance;
            _timeController.OnCurDayChanged += OnCurDayChanged;
            
            _starSystemsController = StarSystemsController.Instance;
            _starSystemsController.OnStarSystemActiveChanged += OnStarSystemActiveChanged;
            
            UpdateShipIconActive();
            if ( State.CurMode == MetaAiShipMode.Moving ) {
                UpdateRotation();
            } 
            UpdateSystems();
        }

        void UpdateSystems() {
            if ( !string.IsNullOrEmpty(State.CurSystemId) ) {
                _curSystem = _starSystemsManager.GetStarSystem(State.CurSystemId);
            }
            if ( !string.IsNullOrEmpty(State.DestSystemId) ) {
                _destSystem = _starSystemsManager.GetStarSystem(State.DestSystemId);
            }
        }

        void UpdateShipIconActive() {
            ShipIcon.enabled = (State.CurMode == MetaAiShipMode.Moving);
        }

        void UpdateRotation() {
            if ( State.CurMode != MetaAiShipMode.Moving ) {
                Debug.LogError("Unsupported scenario");
                return;
            }
            var prevSystem = _starSystemsManager.GetStarSystem(State.CurSystemId);
            var nextSystem = _starSystemsManager.GetStarSystem(State.DestSystemId);

            transform.rotation = Quaternion.Euler(0, 0,
                Vector2.SignedAngle(new Vector3(0, 1), nextSystem.transform.position - prevSystem.transform.position));
        }

        void OnCurDayChanged(int curDay) {
            if ( curDay == State.DestDay ) {
                _aiShipManager.NextStep(this);
                UpdateShipIconActive();
                UpdateSystems();
                switch ( State.CurMode ) {
                    case MetaAiShipMode.Moving: {
                        UpdateRotation();
                        break;
                    }
                    case MetaAiShipMode.Stationary: {
                        break;
                    }
                    default: {
                        Debug.LogErrorFormat("Unsupported MetaAiShipMode '{0}'", State.CurMode.ToString());
                        break;
                    }
                }
            }
        }

        void OnStarSystemActiveChanged(string starSystemId, bool isActive) {
            if ( !isActive && (State.CurMode == MetaAiShipMode.Stationary) && (State.CurSystemId == starSystemId) ) {
                _aiShipManager.Kill(this);
            }
        }
    }
}
