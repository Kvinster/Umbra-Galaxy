using UnityEngine;

using System;

using STP.Behaviour.Starter;
using STP.Common;
using STP.State;
using STP.State.Meta;

using RSG;

namespace STP.Behaviour.Meta {
    public sealed class PlayerShipMovementController : BaseMetaComponent {
        public enum State {
            Idle,
            Selected,
            Moving
        }
        
        MetaTimeManager    _timeManager;
        StarSystemsManager _starSystemsManager;
        
        int           _pathStartDay;
        int           _pathEndDay;
        Promise<bool> _movePromise;
        bool          _interrupt;

        StarSystemPath _curPath;
        int            _nextNodeIndex;

        BaseStarSystem _destSystem;
        BaseStarSystem _curSystem;
        State          _curState = State.Idle;

        public BaseStarSystem DestSystem {
            get => _destSystem;
            private set {
                if ( _destSystem == value ) {
                    return;
                }
                _destSystem = value;
                OnDestSystemChanged?.Invoke(_destSystem);
            }
        }

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

        public State CurState { 
            get => _curState;
            private set {
                if ( _curState == value ) {
                    return;
                }
                _curState = value;
                OnCurStateChanged?.Invoke(_curState);
            }
        }

        public event Action<string>         OnCurSystemChanged;
        public event Action<State>          OnCurStateChanged;
        public event Action<BaseStarSystem> OnDestSystemChanged;

        BaseStarSystem NextSystem =>
            ((_curPath != null) ? _starSystemsManager.GetStarSystem(_curPath.Path[_nextNodeIndex]) : null);

        void Update() {
            if ( CurState != State.Moving ) {
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
            CurState = State.Idle;
        }

        public void TrySelect(BaseStarSystem destSystem) {
            if ( (CurState != State.Idle) && (CurState != State.Selected) ) {
                Debug.LogErrorFormat("Invalid state '{0}'", CurState.ToString());
                return;
            }
            DestSystem = destSystem;
            CurState   = State.Selected;
        }

        public IPromise<bool> Move() {
            if ( !CanMoveTo(DestSystem, false) ) {
                return Promise<bool>.Rejected(new Exception($"Can't move to {DestSystem.Id}"));
            }
            if ( CurState != State.Selected ) {
                Debug.LogErrorFormat("Invalid state '{0}'", CurState.ToString());
                return Promise<bool>.Rejected(new Exception($"Invalid state '{CurState.ToString()}'"));
            }
            _curPath       = StarSystemsController.Instance.GetPath(CurSystem.Id, DestSystem.Id);
            _nextNodeIndex = 1;
            DestSystem = DestSystem;
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
            CurState = State.Moving;
            return _movePromise;
        }

        public void InterruptMoving() {
            if ( !IsMoving ) {
                Debug.LogError("Can't interrupt: not moving");
                return;
            }
            _interrupt = true;
        }
        
        bool CanMoveTo(BaseStarSystem destSystem, bool silent = true) {
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
            return true;
        }

        void OnTimePausedChanged(bool isPaused) {
            if ( (CurState == State.Moving) && isPaused && (_timeManager.CurDay == _pathEndDay) ) {
                var nextSystem = NextSystem;
                transform.position = nextSystem.transform.position;
                CurSystem = nextSystem;
                if ( nextSystem == DestSystem ) {
                    FinishMovement(true);
                } else {
                    ++_nextNodeIndex;
                    nextSystem = NextSystem;
                    if ( !_interrupt && ((nextSystem.Type != StarSystemType.Shard) ||
                          StarSystemsController.Instance.GetShardSystemActive(nextSystem.Id)) ) {
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
            _interrupt   = false;
            CurState = State.Idle;
        }
    }
}
