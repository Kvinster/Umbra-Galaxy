using System;

namespace STP.Utils {
    public class HRTime {
        DateTime _timestamp;

        public HRTime SetSeconds(float seconds) {
            _timestamp = DateTime.MinValue.AddSeconds(seconds);
            return this;
        }
        
        public string GetHMS() {
            var diff  = _timestamp - DateTime.MinValue;
            var hours = diff.Days * 24 + diff.Hours;
            return $"{hours}:{diff.Minutes}:{diff.Seconds}";
        }
        
        public string GetMS() {
            var diff = _timestamp - DateTime.MinValue;
            var minutes = (diff.Days*24 + diff.Hours)*60 + diff.Minutes;
            return $"{minutes}:{diff.Seconds}";
        }
        
        public string GetSM() {
            var diff = _timestamp - DateTime.MinValue;
            var seconds = ((diff.Days * 24 + diff.Hours) * 60 + diff.Minutes) * 60 + diff.Seconds;
            return $"{seconds}:{diff.Milliseconds:000}";
        }
    }
}