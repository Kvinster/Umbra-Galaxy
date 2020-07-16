using UnityEngine;

using System;

using STP.State.Meta;

namespace STP.Behaviour.Meta {
    public sealed class MetaTimeManager : MonoBehaviour {
        const float DayToSec = 2f;

        float _timer   = 0f;
        int   _destDay = 0;

        bool _isPaused = true;
        public bool IsPaused {
            get => _isPaused;
            private set {
                if ( _isPaused == value ) {
                    Debug.LogWarning("Redundant IsPaused setting");
                    return;
                }
                _isPaused = value;
                OnPausedChanged?.Invoke(_isPaused);
                if ( !_isPaused ) {
                    _timer = 0f;
                }
            }
        }
        
        public int CurDay {
            get => TimeController.Instance.CurDay;
            private set => TimeController.Instance.CurDay = value;
        }

        public float DayProgress {
            get {
                if ( IsPaused ) {
                    return 1f;
                }
                return _timer / DayToSec;
            }
        }

        public event Action<bool> OnPausedChanged;

        void Update() {
            if ( IsPaused ) {
                return;
            }
            _timer += Time.deltaTime;
            if ( _timer >= DayToSec ) {
                _timer -= DayToSec;
                ++CurDay;
                if ( CurDay >= _destDay ) {
                    IsPaused = true;
                }
            }
        }

        public void Unpause(int destDay) {
            if ( !IsPaused ) {
                Debug.LogError("Already unpaused");
                return;
            }
            if ( destDay <= CurDay ) {
                Debug.LogErrorFormat("Dest day '{0}' is equal or before cur day '{1}'", destDay, CurDay);
                return;
            }
            _destDay = destDay;
            IsPaused = false;
        }
    }
}
