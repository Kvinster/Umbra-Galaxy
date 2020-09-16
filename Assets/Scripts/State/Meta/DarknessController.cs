using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Meta;
using STP.Common;

using Random = UnityEngine.Random;

namespace STP.State.Meta {
    public sealed class DarknessController : BaseStateController {
        public const int DarknessHitTime = 5;

        readonly DarknessControllerState _state = new DarknessControllerState();

        readonly TimeController        _timeController;
        readonly StarSystemsController _starSystemsController;
        readonly ProgressController    _progressController;

        DarknessInfoHolder _darknessInfoHolder;

        List<string> ThreatenedSystemIds => _state.ThreatenedSystemIds;

        public event Action<string, bool> OnStarSystemAttack;
        public event Action<string, bool> OnStarSystemThreatChanged;

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

        public void CancelThreat(string threatenedSystemId) {
            if ( !_state.ThreatenedSystemIds.Remove(threatenedSystemId) ) {
                Debug.LogErrorFormat("Can't cancel threat on star system '{0}' — star system not threatened");
                return;
            }
            OnStarSystemThreatChanged?.Invoke(threatenedSystemId, false);
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
            if ( ThreatenedSystemIds.Contains(factionSystemId) ) {
                return true;
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
            if ( ThreatenedSystemIds.Count > 0 ) {
                var ssc = _starSystemsController;
                for ( var i = ThreatenedSystemIds.Count - 1; i >= 0; i-- ) {
                    var starSystemId = ThreatenedSystemIds[i];
                    var chance       = ssc.GetFactionSystemSurvivalChance(starSystemId);
                    var roll         = Random.Range(0, 100);
                    if ( roll > chance ) {
                        ssc.SetFactionSystemActive(starSystemId, false);
                        _progressController.OnStarSystemCaptured(starSystemId);
                        OnStarSystemAttack?.Invoke(starSystemId, true);
                    } else {
                        OnStarSystemAttack?.Invoke(starSystemId, false);
                    }
                    OnStarSystemThreatChanged?.Invoke(starSystemId, false);
                }
                ThreatenedSystemIds.Clear();
            }
            if ( (curDay > 0) && ((curDay % DarknessHitTime) == 0) ) {
                var ssc = _starSystemsController;
                foreach ( var path in _darknessInfoHolder.DarknessPaths ) {
                    foreach ( var starSystem in path.Path ) {
                        if ( ssc.GetFactionSystemActive(starSystem) ) {
                            _state.ThreatenedSystemIds.Add(starSystem);
                            OnStarSystemThreatChanged?.Invoke(starSystem, true);
                            break;
                        }
                    }
                }
            }
        }
    }
}
