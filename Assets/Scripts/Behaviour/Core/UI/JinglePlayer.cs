using UnityEngine;

using STP.Core;
using STP.Utils.GameComponentAttributes;

using DG.Tweening;

namespace STP.Behaviour.Core.UI {
	public sealed class JinglePlayer : MonoBehaviour {
		[Header("Parameters")]
		public float FadeVolume = -60f;
		public float FadeDuration     = 0.5f;
		public float FadeBackDuration = 0.5f;
		public float FadeBackOffset   = 0.5f;
		[Header("Dependencies")]
		[NotNull] public AudioSource AudioSource;

		float _cachedMusicVolume;

		Sequence _anim;

		SettingsController SettingsController => GameController.Instance.SettingsController;

		void OnDestroy() {
			if ( _anim != null ) {
				_anim.Kill(true);
				SettingsController.MasterVolume          = _cachedMusicVolume;
				SettingsController.OverrideJinglesVolume = false;
			}
		}

		public void Play() {
			if ( _anim != null ) {
				Debug.LogError("Double start not supported!");
				return;
			}
			_cachedMusicVolume                       = SettingsController.MusicVolume;
			SettingsController.OverrideJinglesVolume = true;
			_anim = DOTween.Sequence()
				.Append(DOTween.To(() => SettingsController.MusicVolume, x => SettingsController.MusicVolume                                           = x, Mathf.Min(_cachedMusicVolume, FadeVolume), FadeDuration))
				.Insert(AudioSource.clip.length - FadeBackOffset, DOTween.To(() => SettingsController.MusicVolume, x => SettingsController.MusicVolume = x, _cachedMusicVolume, FadeBackDuration))
				.SetUpdate(true)
				.OnComplete(() => SettingsController.OverrideJinglesVolume = false);
			AudioSource.Play();
		}
	}
}
