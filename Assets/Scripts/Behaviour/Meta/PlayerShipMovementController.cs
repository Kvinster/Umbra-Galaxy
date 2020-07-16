using UnityEngine;

using System;

using STP.Behaviour.Starter;
using STP.Common;
using STP.State;

using RSG;

namespace STP.Behaviour.Meta {
    public sealed class PlayerShipMovementController : BaseMetaComponent {
        MetaTimeManager    _timeManager;
        StarSystemsManager _starSystemsManager;
        
        int           _pathStartDay;
        int           _pathEndDay;
        Promise<bool> _movePromise;

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
                PlayerState.Instance.CurSystemId = _curSystem.Id;
                OnCurSystemChanged?.Invoke(_curSystem.Id);
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
            
            var curSystemId = PlayerState.Instance.CurSystemId;
            CurSystem = starter.StarSystemsManager.GetStarSystem(curSystemId);
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
                    Debug.LogErrorFormat("Current and destination system are the same system '{0}'", CurSystem.Id);
                }
                return false;
            }
            if ( !_timeManager.IsPaused ) {
                if ( !silent ) {
                    Debug.LogError("Can't start movement when the time is moving");
                }
                return false;
            }
            var path = StarSystemsController.Instance.GetPath(CurSystem.Id, destSystem.Id);
            if ( path == null ) {
                return false;
            }
            if ( path.PathLength > PlayerState.Instance.Fuel ) {
                return false;
            }
            for ( var i = 1; i < path.Path.Count - 1; i++ ) {
                var starSystem = path.Path[i];
                if ( (_starSystemsManager.GetStarSystem(starSystem).Type == StarSystemType.Faction) &&
                     !StarSystemsController.Instance.GetFactionSystemActive(starSystem) ) {
                    return false;
                }
            }
            return true;
        }

        public IPromise<bool> MoveTo(BaseStarSystem destSystem) {
            if ( !CanMoveTo(destSystem, false) ) {
                return Promise<bool>.Rejected(new Exception($"Can't move to {destSystem.Id}"));
            }
            _curPath       = StarSystemsController.Instance.GetPath(CurSystem.Id, destSystem.Id);
            _nextNodeIndex = 1;
            DestSystem = destSystem;
            var nextDistance =
                StarSystemsController.Instance.GetDistance(CurSystem.Id, _curPath.Path[_nextNodeIndex]);
            PlayerState.Instance.Fuel -= nextDistance;
            _pathStartDay = _timeManager.CurDay;
            _pathEndDay   = _pathStartDay + nextDistance;
            _timeManager.OnPausedChanged += OnTimePausedChanged;
            _movePromise = new Promise<bool>();
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
                    FinishMovement(true);
                } else {
                    ++_nextNodeIndex;
                    nextSystem = NextSystem;
                    if ( (nextSystem.Type != StarSystemType.Shard) ||
                         StarSystemsController.Instance.GetShardSystemActive(nextSystem.Id) ) {
                        var nextDistance = StarSystemsController.Instance.GetDistance(CurSystem.Id, nextSystem.Id);
                        _pathStartDay = _timeManager.CurDay;
                        _pathEndDay   = _pathStartDay + nextDistance;
                        PlayerState.Instance.Fuel -= nextDistance;
                        _timeManager.Unpause(_pathEndDay);
                        transform.rotation = Quaternion.Euler(0, 0,
                            Vector2.SignedAngle(new Vector3(0, 1), NextSystem.transform.position - transform.position));
                    } else {
                        FinishMovement(false);
                    }
                }
            }
        }

        void FinishMovement(bool success) {
            DestSystem = null;
            _timeManager.OnPausedChanged -= OnTimePausedChanged;
            _movePromise.Resolve(success);
            _movePromise = null;
        }
    }
}
