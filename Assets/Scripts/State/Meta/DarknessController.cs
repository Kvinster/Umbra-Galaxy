using UnityEngine;

using STP.Behaviour.Meta;

namespace STP.State.Meta {
    public sealed class DarknessController {
        public const int DarknessHitTime = 5; 
        
        static DarknessController _instance;
        public static DarknessController Instance {
            get {
                TryCreate();
                return _instance;
            }
        }

        readonly DarknessInfoHolder _darknessInfoHolder;
        readonly TimeController     _timeController;

        DarknessController() {
            _darknessInfoHolder = Resources.Load<DarknessInfoHolder>(DarknessInfoHolder.ResourcesPath);

            _timeController = TimeController.Instance;
            _timeController.OnCurDayChanged += OnCurDayChanged;
        }

        void OnCurDayChanged(int curDay) {
            if ( (curDay > 0) && ((curDay % DarknessHitTime) == 0) ) {
                var ssc = StarSystemsController.Instance;
                foreach ( var path in _darknessInfoHolder.DarknessPaths ) {
                    foreach ( var starSystem in path.Path ) {
                        if ( ssc.GetFactionSystemActive(starSystem) ) {
                            var chance = ssc.GetFactionSystemSurvivalChance(starSystem);
                            var roll   = Random.Range(0, 100);
                            if ( roll > chance ) {
                                ssc.SetFactionSystemActive(starSystem, false);
                                ProgressController.Instance.OnStarSystemCaptured(starSystem);
                            }
                            break;
                        }
                    }
                }
            }
        }

        [RuntimeInitializeOnLoadMethod]
        static void TryCreate() {
            if ( _instance == null ) {
                _instance = new DarknessController();
            }
        }
    }
}
