using System;
using System.Collections.Generic;

using STP.Behaviour.Core.Enemy;

namespace STP.Behaviour.Core {
	public class GeneratorsWatcher {
		public static event Action GeneratorsCountChanged;

		public static List<Generator> MainGenerators = new List<Generator>();

		public static void TryAddGenerator(Generator generator) {
			if ( !generator.IsMainGenerator ) {
				return;
			}
			MainGenerators.Add(generator);
			GeneratorsCountChanged?.Invoke();
		}

		public static void RemoveGenerator(Generator generator) {
			MainGenerators.Remove(generator);
			GeneratorsCountChanged?.Invoke();
		}
	}
}