using System.Collections.Generic;

using STP.Gameplay;
using STP.Utils;

namespace STP.Behaviour.Core {
    public abstract class CoreComponent : GameComponent {
		public static readonly HashSet<CoreComponent> Instances = new HashSet<CoreComponent>();
    
		protected void OnEnable() {
			Instances.Add(this);
		}

		protected void OnDisable() {
			Instances.Remove(this);
		}


        public abstract void Init(CoreStarter starter);
    }
}