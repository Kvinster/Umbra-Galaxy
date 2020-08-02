using UnityEngine;

using STP.Behaviour.Meta;

namespace STP.State.Meta {
    public sealed class ShardsActiveController {
        static ShardsActiveController _instance;
        public static ShardsActiveController Instance {
            get {
                TryCreate();
                return _instance;
            }
        }

        TimeController        _timeController;
        StarSystemsController _starSystemsController;
        ShardsActiveSetup     _shardsActiveSetup;

        ShardsActiveController Init() {
            _shardsActiveSetup = Resources.Load<ShardsActiveSetup>(ShardsActiveSetup.ResourcesPath);
            if ( !_shardsActiveSetup ) {
                Debug.LogError("Can't load ShardsActiveSetup from Resources");
                return null;
            }

            _timeController = TimeController.Instance;
            _timeController.OnCurDayChanged += OnCurDayChanged;

            _starSystemsController = StarSystemsController.Instance;
            var curDay = _timeController.CurDay;
            foreach ( var shardActiveSetup in _shardsActiveSetup.ShardActiveSetups ) {
                foreach ( var activePeriod in shardActiveSetup.ActivePeriods ) {
                    if ( (activePeriod.ActivationDay > curDay) ||
                         (activePeriod.ActivationDay + activePeriod.ActivationPeriod < curDay) ) {
                        _starSystemsController.SetShardSystemActive(shardActiveSetup.ShardId, false);
                    } else {
                        _starSystemsController.SetShardSystemActive(shardActiveSetup.ShardId, true);
                        break;
                    }
                }
            }
            
            return this;
        }

        void OnCurDayChanged(int curDay) {
            foreach ( var shardActiveSetup in _shardsActiveSetup.ShardActiveSetups ) {
                foreach ( var activePeriod in shardActiveSetup.ActivePeriods ) {
                    if ( activePeriod.ActivationDay == curDay ) {
                        _starSystemsController.SetShardSystemActive(shardActiveSetup.ShardId, true);
                        break;
                    }
                    if ( activePeriod.ActivationDay + activePeriod.ActivationPeriod == curDay ) {
                        _starSystemsController.SetShardSystemActive(shardActiveSetup.ShardId, false);
                        break;
                    }
                }
            }
        }

        [RuntimeInitializeOnLoadMethod]
        static void TryCreate() {
            if ( _instance == null ) {
                _instance = new ShardsActiveController().Init();
            }
        }
    }
}
