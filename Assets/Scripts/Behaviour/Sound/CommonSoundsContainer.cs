using UnityEngine;

namespace STP.Behaviour.Sound {
	[CreateAssetMenu(fileName = nameof(CommonSoundsContainer), menuName = "ScriptableObjects/" + nameof(CommonSoundsContainer))]
	public sealed class CommonSoundsContainer : ScriptableObject {
		public AudioClip UiClickClip;
	}
}
