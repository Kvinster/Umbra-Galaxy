using UnityEngine;

namespace STP.Utils {
    public class Timer {
        public float TimePassed { get; private set; }
        public float Interval   { get; private set; }
        public float TimeLeft   => Interval - TimePassed;

        public float NormalizedProgress => (Interval > float.Epsilon) ? Mathf.Clamp01(TimePassed / Interval) : 0.0f;

        public void Start(float interval, float passedTime = 0f) {
            Interval   = interval;
            TimePassed = passedTime;
        }

        public void Reset() {
            Reset(Interval);
        }

        public void Reset(float interval, float passedTime = 0f) {
            Interval   = interval;
            TimePassed = passedTime;
        }

        public bool DeltaTick() {
            return Tick(Time.deltaTime);
        }

        public bool Tick(float deltaTime) {
            TimePassed += deltaTime;
            var passedInterval = TimePassed > Interval;
            if ( passedInterval ) {
                TimePassed -= Interval;
            }
            return passedInterval;
        }

        public void Stop() {
            Start(float.MaxValue);
        }
    }
}