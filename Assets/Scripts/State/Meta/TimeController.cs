using System;

namespace STP.State.Meta {
    public sealed class TimeController : BaseStateController {
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

        public event Action<int> OnCurDayChanged;
    }
}
