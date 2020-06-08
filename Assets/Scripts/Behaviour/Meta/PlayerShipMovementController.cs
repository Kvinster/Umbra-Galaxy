using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.State;

using RSG;

namespace STP.Behaviour.Meta {
    public sealed class PlayerShipMovementController : BaseMetaComponent {
        StarSystemsGraphInfo _graphInfo;
        MetaTimeManager      _timeManager;
        
        int     _pathStartDay;
        int     _pathEndDay;
        Promise _movePromise;

        readonly Dictionary<(string, string), int> _distanceCache = new Dictionary<(string, string), int>();
        
        public BaseStarSystem CurSystem  { get; private set; }
        public BaseStarSystem DestSystem { get; private set; }

        public bool IsMoving => (DestSystem && !_timeManager.IsPaused);

        void Update() {
            if ( !DestSystem ) {
                return;
            }
            var progress = (_timeManager.CurDay - _pathStartDay + _timeManager.DayProgress) /
                           (_pathEndDay - _pathStartDay);
            transform.position = Vector2.Lerp(CurSystem.transform.position, DestSystem.transform.position, progress);
        }
        
        protected override void InitInternal(MetaStarter starter) {
            _graphInfo   = starter.StarSystemsGraphInfo;
            _timeManager = starter.TimeManager;

            CurSystem = starter.StartStarSystem;
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
            var distance = GetDistance(CurSystem.Name, destSystem.Name);
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
            var distance = GetDistance(CurSystem.Name, destSystem.Name);
            PlayerState.Instance.Fuel -= distance;
            DestSystem    = destSystem;
            _pathStartDay = _timeManager.CurDay;
            _pathEndDay   = _pathStartDay + distance;
            _timeManager.OnPausedChanged += OnTimePausedChanged;
            _movePromise = new Promise();
            _timeManager.Unpause(_pathEndDay);
            transform.rotation =
                Quaternion.Euler(0, 0, Vector2.SignedAngle(new Vector3(0, 1), DestSystem.transform.position - transform.position));
            return _movePromise;
        }

        void OnTimePausedChanged(bool isPaused) {
            if ( DestSystem && isPaused && (_timeManager.CurDay == _pathEndDay) ) {
                transform.position = DestSystem.transform.position;
                CurSystem  = DestSystem;
                DestSystem = null;
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
