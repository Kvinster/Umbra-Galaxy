using System.Collections.Generic;

using STP.Utils;

namespace STP.View {
    public abstract class CoreBehaviour : GameBehaviour {
		public static readonly HashSet<CoreBehaviour> Instances = new HashSet<CoreBehaviour>();
    
		protected void OnEnable() {
			Instances.Add(this);
		}

		protected void OnDisable() {
			Instances.Remove(this);
		}


        public abstract void Init(CoreStarter starter);
    }
}