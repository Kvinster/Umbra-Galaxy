namespace STP.Utils {
    public class Timer {
        public float TimePassed {get; private set;}
        public float Interval   {get; private set;}
        public float LeftTime   => Interval - TimePassed;

        public void Start(float interval, float passedTime = 0f) {
            Interval   = interval;
            TimePassed = passedTime;
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