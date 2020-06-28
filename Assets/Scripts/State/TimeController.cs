using System;

namespace STP.State {
    public sealed class TimeController {
        static TimeController _instance;
        public static TimeController Instance => _instance ?? (_instance = new TimeController());

        readonly TimeState _state = new TimeState();

        public int CurDay {
            get => _state.CurDay;
            set {
                if ( _state.CurDay == value ) {
                    return;
                }
                _state.CurDay = value;
                OnCurDayChanged?.Invoke(_state.CurDay);
            }
        }

        public Action<int> OnCurDayChanged;
    }
}
