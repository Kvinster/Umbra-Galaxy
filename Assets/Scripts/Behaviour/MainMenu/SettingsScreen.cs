using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.Core;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.MainMenu {
	public class SettingsScreen : GameComponent, IScreen {
		const string MasterVolumeId = SettingsController.MasterVolumeId;
		const string MusicVolumeId  = SettingsController.MusicVolumeId;
		const string SfxVolumeId    = SettingsController.SfxVolumeId;

		[Header("Parameters")]
		public float VolMin = -40f;
		public float VolMax = 0f;
		[Header("Dependencies")]
		[NotNull] public Slider      MasterVolumeSlider;
		[NotNull] public Slider      MusicVolumeSlider;
		[NotNull] public Slider      SfxVolumeSlider;
		[NotNull] public AudioSource SfxTestAudioSource;
		[NotNull] public AudioMixer  AudioMixer;
		[NotNull] public Button      AcceptButton;

		IScreenShower _screenShower;

		SettingsController _settingsController;

		float CurMasterVolume {
			get {
				AudioMixer.GetFloat(MasterVolumeId, out var volume);
				return volume;
			}
		}

		float CurMusicVolume {
			get {
				AudioMixer.GetFloat(MusicVolumeId, out var volume);
				return volume;
			}
		}

		float CurSfxVolume {
			get {
				AudioMixer.GetFloat(SfxVolumeId, out var volume);
				return volume;
			}
		}

		public void Init(MainMenuStarter starter) {
			_settingsController = starter.GameController.SettingsController;
			_screenShower       = starter.ScreensViewController;

			InitVolumeSlider(MasterVolumeSlider);
			InitVolumeSlider(MusicVolumeSlider);
			InitVolumeSlider(SfxVolumeSlider);

			UpdateMasterVolume(_settingsController.MasterVolume);
			UpdateMusicVolume(_settingsController.MusicVolume);
			UpdateSfxVolume(_settingsController.SfxVolume);
		}

		public void Show() {
			MasterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolume);
			MusicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolume);
			SfxVolumeSlider.onValueChanged.AddListener(UpdateSfxVolume);
			AcceptButton.onClick.AddListener(() => {
				ApplyChanges();
				_screenShower.Show<MainScreen>();
			});
			gameObject.SetActive(true);
			UpdateView();
			SfxVolumeSlider.onValueChanged.AddListener(TestSfxVolume);
		}

		public void Hide() {
			MasterVolumeSlider.onValueChanged.RemoveListener(UpdateMasterVolume);
			MusicVolumeSlider.onValueChanged.RemoveListener(UpdateMusicVolume);
			SfxVolumeSlider.onValueChanged.RemoveListener(UpdateSfxVolume);
			SfxVolumeSlider.onValueChanged.RemoveListener(TestSfxVolume);
			AcceptButton.onClick.RemoveAllListeners();
			gameObject.SetActive(false);
		}

		void InitVolumeSlider(Slider slider) {
			slider.minValue = VolMin;
			slider.maxValue = VolMax;
		}

		void UpdateMasterVolume(float value) {
			UpdateVolume(MasterVolumeId, value);
		}

		void UpdateMusicVolume(float value) {
			UpdateVolume(MusicVolumeId, value);
		}

		void UpdateSfxVolume(float value) {
			UpdateVolume(SfxVolumeId, value);
		}

		void TestSfxVolume(float _) {
			if ( SfxTestAudioSource.isPlaying ) {
				return;
			}
			SfxTestAudioSource.Play();
		}

		void UpdateVolume(string id, float value) {
			AudioMixer.SetFloat(id, value);
		}

		void UpdateView() {
			MasterVolumeSlider.value = CurMasterVolume;
			MusicVolumeSlider.value  = CurMusicVolume;
			SfxVolumeSlider.value    = CurSfxVolume;
		}

		void DiscardChanges() {
			UpdateVolume(MasterVolumeId, _settingsController.MasterVolume);
			UpdateVolume(MusicVolumeId, _settingsController.MusicVolume);
			UpdateVolume(SfxVolumeId, _settingsController.SfxVolume);
		}

		void ApplyChanges() {
			_settingsController.MasterVolume = CurMasterVolume;
			_settingsController.MusicVolume  = CurMusicVolume;
			_settingsController.SfxVolume    = CurSfxVolume;
		}
	}
}