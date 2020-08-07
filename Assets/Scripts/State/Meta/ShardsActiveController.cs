using UnityEngine;

using STP.Behaviour.Meta;

namespace STP.State.Meta {
    public sealed class ShardsActiveController : BaseStateController {
        readonly TimeController        _timeController;
        readonly StarSystemsController _starSystemsController;
        
        ShardsActiveSetup _shardsActiveSetup;

        public ShardsActiveController(TimeController timeController, StarSystemsController starSystemsController) {
            _timeController        = timeController;
            _starSystemsController = starSystemsController;
        }

        public override void Init() {
            _shardsActiveSetup = Resources.Load<ShardsActiveSetup>(ShardsActiveSetup.ResourcesPath);
            if ( !_shardsActiveSetup ) {
                Debug.LogError("Can't load ShardsActiveSetup from Resources");
                return;
            }
            
            _timeController.OnCurDayChanged += OnCurDayChanged;
            
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
    }
}
