using UnityEngine;

using System;

using STP.Behaviour.Meta;
using STP.Common;

using Random = UnityEngine.Random;

namespace STP.State.Meta {
    public sealed class DarknessController : BaseStateController {
        public const int DarknessHitTime = 5; 

        readonly TimeController        _timeController;
        readonly StarSystemsController _starSystemsController;
        readonly ProgressController    _progressController;
        
        DarknessInfoHolder _darknessInfoHolder;

        public event Action<string, bool> OnStarSystemAttack;

        public DarknessController(TimeController timeController, StarSystemsController starSystemsController,
            ProgressController progressController) {
            _timeController        = timeController;
            _starSystemsController = starSystemsController;
            _progressController    = progressController;
        }

        public override void Init() {
            _darknessInfoHolder = Resources.Load<DarknessInfoHolder>(DarknessInfoHolder.ResourcesPath);

            _timeController.OnCurDayChanged += OnCurDayChanged;
        }
        
        public bool IsFactionSystemNextToHit(string factionSystemId) {
            if ( _starSystemsController.GetStarSystemType(factionSystemId) != StarSystemType.Faction ) {
                Debug.LogErrorFormat("Star system '{0}' is not a faction system", factionSystemId);
                return false;
            }
            if ( !_starSystemsController.GetFactionSystemActive(factionSystemId) ) {
                Debug.LogErrorFormat("Faction system '{0}' is not active", factionSystemId);
                return false;
            }
            var ssc = _starSystemsController;
            foreach ( var path in _darknessInfoHolder.DarknessPaths ) {
                foreach ( var starSystem in path.Path ) {
                    if ( ssc.GetFactionSystemActive(starSystem) ) {
                        if ( starSystem == factionSystemId ) {
                            return true;
                        }
                        break;
                    }
                }
            }
            return false;
        }

        void OnCurDayChanged(int curDay) {
            if ( (curDay > 0) && ((curDay % DarknessHitTime) == 0) ) {
                var ssc = _starSystemsController;
                foreach ( var path in _darknessInfoHolder.DarknessPaths ) {
                    foreach ( var starSystem in path.Path ) {
                        if ( ssc.GetFactionSystemActive(starSystem) ) {
                            var chance = ssc.GetFactionSystemSurvivalChance(starSystem);
                            var roll   = Random.Range(0, 100);
                            if ( roll > chance ) {
                                ssc.SetFactionSystemActive(starSystem, false);
                                _progressController.OnStarSystemCaptured(starSystem);
                                OnStarSystemAttack?.Invoke(starSystem, true);
                            } else {
                                OnStarSystemAttack?.Invoke(starSystem, false);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
