using STP.Utils;
using STP.Utils.GameComponentAttributes;
using UnityEngine;
using UnityEngine.VFX;

namespace STP.Behaviour.Core {
	[RequireComponent(typeof(VisualEffect))]
	public class VisualEffectSortingOrderSetter : GameComponent {
		[NotNull] public Renderer Renderer;
		
		public int    SortingOrder;
		public string SortingLayer;

		public void Reset() {
			Renderer = gameObject.GetComponent<Renderer>();
		}

		public void Start() {
			Renderer.sortingOrder     = SortingOrder;
			Renderer.sortingLayerName = SortingLayer;
		}
	}
}