using UnityEngine;

using System;

using STP.Behaviour.Starter;
using STP.State;

using RSG;

namespace STP.Behaviour.Meta {
    public sealed class PlayerShipMovementController : BaseMetaComponent {
        MetaTimeManager    _timeManager;
        StarSystemsManager _starSystemsManager;
        
        int     _pathStartDay;
        int     _pathEndDay;
        Promise _movePromise;

        StarSystemPath _curPath;
        int            _nextNodeIndex;

        BaseStarSystem _curSystem;

        public BaseStarSystem DestSystem { get; private set; }

        public BaseStarSystem CurSystem {
            get => _curSystem;
            private set {
                if ( _curSystem == value ) {
                    return;
                }
                _curSystem = value;
                OnCurSystemChanged?.Invoke(_curSystem.Name);
            }
        }

        public bool IsMoving => (DestSystem && !_timeManager.IsPaused);

        public event Action<string> OnCurSystemChanged;

        BaseStarSystem NextSystem =>
            ((_curPath != null) ? _starSystemsManager.GetStarSystem(_curPath.Path[_nextNodeIndex]) : null);

        void Update() {
            if ( !DestSystem ) {
                return;
            }
            var progress = (_timeManager.CurDay - _pathStartDay + _timeManager.DayProgress) /
                           (_pathEndDay - _pathStartDay);
            transform.position = Vector2.Lerp(CurSystem.transform.position, NextSystem.transform.position, progress);
        }
        
        protected override void InitInternal(MetaStarter starter) {
            _timeManager        = starter.TimeManager;
            _starSystemsManager = starter.StarSystemsManager;

            var curSystemName = PlayerState.Instance.CurSystem;
            CurSystem = starter.StarSystemsManager.GetStarSystem(curSystemName);
            transform.position = CurSystem.transform.position;
        }

        public bool CanMoveTo(BaseStarSystem destSystem, bool silent = true) {
            if ( !destSystem ) {
                if ( !silent ) {
                    Debug.LogError("Destination systems is null");
                }
                return false;
            }
            if ( CurSystem == destSystem ) {
                if ( !silent ) {
                    Debug.LogErrorFormat("Current and destination system are the same system '{0}'", CurSystem.Name);
                }
                return false;
            }
            if ( !_timeManager.IsPaused ) {
                if ( !silent ) {
                    Debug.LogError("Can't start movement when the time is moving");
                }
                return false;
            }
            var path = StarSystemsController.Instance.GetPath(CurSystem.Name, destSystem.Name);
            if ( path == null ) {
                return false;
            }
            if ( path.PathLength > PlayerState.Instance.Fuel ) {
                return false;
            }
            for ( var i = 1; i < path.Path.Count; i++ ) {
                var starSystem = path.Path[i];
                if ( !StarSystemsController.Instance.GetStarSystemActive(starSystem) ) {
                    return false;
                }
            }
            return true;
        }

        public IPromise MoveTo(BaseStarSystem destSystem) {
            if ( !CanMoveTo(destSystem, false) ) {
                return Promise.Rejected(new Exception($"Can't move to {destSystem.Name}"));
            }
            _curPath       = StarSystemsController.Instance.GetPath(CurSystem.Name, destSystem.Name);
            _nextNodeIndex = 1;
            DestSystem = destSystem;
            var nextDistance =
                StarSystemsController.Instance.GetDistance(CurSystem.Name, _curPath.Path[_nextNodeIndex]);
            PlayerState.Instance.Fuel -= nextDistance;
            _pathStartDay = _timeManager.CurDay;
            _pathEndDay   = _pathStartDay + nextDistance;
            _timeManager.OnPausedChanged += OnTimePausedChanged;
            _movePromise = new Promise();
            _timeManager.Unpause(_pathEndDay);
            transform.rotation =
                Quaternion.Euler(0, 0,
                    Vector2.SignedAngle(new Vector3(0, 1), NextSystem.transform.position - transform.position));
            return _movePromise;
        }

        void OnTimePausedChanged(bool isPaused) {
            if ( DestSystem && isPaused && (_timeManager.CurDay == _pathEndDay) ) {
                var nextSystem = NextSystem;
                transform.position = nextSystem.transform.position;
                CurSystem = nextSystem;
                if ( nextSystem == DestSystem ) {
                    FinishMovement();
                } else {
                    ++_nextNodeIndex;
                    nextSystem = NextSystem;
                    if ( StarSystemsController.Instance.GetStarSystemActive(nextSystem.Name) ) {
                        var nextDistance = StarSystemsController.Instance.GetDistance(CurSystem.Name, nextSystem.Name);
                        _pathStartDay = _timeManager.CurDay;
                        _pathEndDay   = _pathStartDay + nextDistance;
                        PlayerState.Instance.Fuel -= nextDistance;
                        _timeManager.Unpause(_pathEndDay);
                        transform.rotation = Quaternion.Euler(0, 0,
                            Vector2.SignedAngle(new Vector3(0, 1), NextSystem.transform.position - transform.position));
                    } else {
                        FinishMovement();
                    }
                }
            }
        }

        void FinishMovement() {
            DestSystem = null;
            _timeManager.OnPausedChanged -= OnTimePausedChanged;
            _movePromise.Resolve();
            _movePromise = null;
        }
    }
}
