using STP.Utils;
using STP.Utils.GameComponentAttributes;
using UnityEngine;
using UnityEngine.VFX;

namespace STP.Behaviour.Core {
	[RequireComponent(typeof(VisualEffect))]
	[ExecuteInEditMode]
	public sealed class VisualEffectSortingOrderSetter : GameComponent {
		[NotNull] public Renderer Renderer;

		public int    SortingOrder;
		public string SortingLayer;

		void Reset() {
			Renderer = gameObject.GetComponent<Renderer>();
		}

		void Start() {
			Renderer.sortingOrder     = SortingOrder;
			Renderer.sortingLayerName = SortingLayer;
		}

		void Update() {
			if ( Application.isPlaying ) {
				return;
			}
			Renderer.sortingOrder     = SortingOrder;
			Renderer.sortingLayerName = SortingLayer;
		}
	}
}