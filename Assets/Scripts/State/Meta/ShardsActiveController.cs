using UnityEngine;

using STP.Behaviour.Meta;
using STP.Common;

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

        public int GetShardActiveDaysRemaining(string shardSystemId) {
            if ( _starSystemsController.GetStarSystemType(shardSystemId) != StarSystemType.Shard ) {
                Debug.LogErrorFormat("Star system '{0}' is not a shard", shardSystemId);
                return -1;
            }
            if ( !_starSystemsController.GetShardSystemActive(shardSystemId) ) {
                Debug.LogErrorFormat("Shard system '{0}' is not active", shardSystemId);
                return -1;
            }
            var curDay = _timeController.CurDay;
            foreach ( var shardActiveSetup in _shardsActiveSetup.ShardActiveSetups ) {
                if ( shardActiveSetup.ShardId != shardSystemId ) {
                    continue;
                }
                foreach ( var activePeriod in shardActiveSetup.ActivePeriods ) {
                    if ( activePeriod.ActivationDay > curDay ) {
                        continue;
                    }
                    var activationEndDay = activePeriod.ActivationDay + activePeriod.ActivationPeriod;
                    if ( activationEndDay < curDay ) {
                        continue;
                    }
                    return (activationEndDay - curDay);
                }
                Debug.LogErrorFormat("Can't find cur active period for shard '{0}'", shardSystemId);
                return -1;
            }
            Debug.LogErrorFormat("Can't find shard active setup for shard '{0}'", shardSystemId);
            return -1;
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
