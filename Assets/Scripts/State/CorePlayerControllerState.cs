using System.Collections.Generic;

namespace STP.State {
    public class CorePlayerControllerState {
        public HashSet<string> Keys = new HashSet<string>();

        public void Reset() {
            Keys = new HashSet<string>();
        }
    }
}