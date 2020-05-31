using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.State;

using RSG;

namespace STP.Behaviour.Meta {
    public sealed class PlayerShipMovementController : BaseMetaComponent {
        public BaseStarSystem StartSystem;

        StarSystemsGraphInfo _graphInfo;
        MetaTimeManager      _timeManager;

        BaseStarSystem _curSystem;
        BaseStarSystem _destSystem;

        int     _pathStartDay;
        int     _pathEndDay;
        Promise _movePromise;

        readonly Dictionary<(string, string), int> _distanceCache = new Dictionary<(string, string), int>();

        void Update() {
            if ( !_destSystem ) {
                return;
            }
            var progress = (_timeManager.CurDay - _pathStartDay + _timeManager.DayProgress) /
                           (_pathEndDay - _pathStartDay);
            transform.position = Vector2.Lerp(_curSystem.transform.position, _destSystem.transform.position, progress);
        }
        
        protected override void InitInternal(MetaStarter starter) {
            _graphInfo   = starter.StarSystemsGraphInfo;
            _timeManager = starter.TimeManager;

            _curSystem = StartSystem;
        }

        public bool CanMoveTo(BaseStarSystem destSystem, bool silent = true) {
            if ( !destSystem ) {
                if ( !silent ) {
                    Debug.LogError("Destination systems is null");
                }
                return false;
            }
            if ( _curSystem == destSystem ) {
                if ( !silent ) {
                    Debug.LogErrorFormat("Current and destination system are the same system '{0}'", _curSystem.Name);
                }
                return false;
            }
            if ( !_timeManager.IsPaused ) {
                if ( !silent ) {
                    Debug.LogError("Can't start movement when the time is moving");
                }
                return false;
            }
            var distance = GetDistance(_curSystem.Name, destSystem.Name);
            if ( distance <= 0 ) {
                return false;
            }
            if ( distance > PlayerState.Instance.Fuel ) {
                if ( !silent ) {
                    Debug.LogError("Not enough fuel");
                }
                return false;
            }
            return true;
        }

        public IPromise MoveTo(BaseStarSystem destSystem) {
            if ( !CanMoveTo(destSystem, false) ) {
                return Promise.Rejected(new Exception($"Can't move to {destSystem.Name}"));
            }
            var distance = GetDistance(_curSystem.Name, destSystem.Name);
            _destSystem   = destSystem;
            PlayerState.Instance.Fuel -= distance;
            _pathStartDay = _timeManager.CurDay;
            _pathEndDay   = _pathStartDay + distance;
            _timeManager.OnPausedChanged += OnTimePausedChanged;
            _movePromise = new Promise();
            _timeManager.Unpause(_pathEndDay);
            transform.rotation =
                Quaternion.Euler(0, 0, Vector2.SignedAngle(new Vector3(0, 1), _destSystem.transform.position - transform.position));
            return _movePromise;
        }

        void OnTimePausedChanged(bool isPaused) {
            if ( _destSystem && isPaused && (_timeManager.CurDay == _pathEndDay) ) {
                transform.position = _destSystem.transform.position;
                _curSystem  = _destSystem;
                _destSystem = null;
                _timeManager.OnPausedChanged -= OnTimePausedChanged;
                _movePromise.Resolve();
                _movePromise = null;
            }
        }

        int GetDistance(string aStarSystem, string bStarSystem) {
            var pair = (aStarSystem, bStarSystem);
            if ( !_distanceCache.TryGetValue(pair, out var distance) ) {
                distance = _graphInfo.GetDistance(aStarSystem, bStarSystem);
                if ( distance > 0 ) {
                    _distanceCache.Add(pair, distance);
                }
            }
            return distance;
        }
    }
}
