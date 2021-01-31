using UnityEngine.Audio;
using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.Core;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.MainMenu {
	public class SettingsScreen : BaseMainMenuComponent {
		const string MasterVolume = "masterVol";

		const float VolMin = -80;
		const float VolMax = 20;

		float CurMasterVolume {
			get {
				AudioMixer.GetFloat(MasterVolume, out var volume);
				return volume;
			}
		}

		[NotNull] public Slider     VolumeSlider;
		[NotNull] public AudioMixer AudioMixer;
		[NotNull] public Button     BackButton;
		[NotNull] public Button     ApplySettingsButton;

		MainMenuManager _mainMenuManager;

		SettingsController _settingsController;

		protected override void InitInternal(MainMenuStarter starter) {
			_settingsController = starter.GameController.SettingsController;
			_mainMenuManager    = starter.MainMenuManager;

			VolumeSlider.minValue = VolMin;
			VolumeSlider.maxValue = VolMax;
			UpdateVolume(_settingsController.MasterVolume);
		}

		public void Show() {
			VolumeSlider.onValueChanged.AddListener(UpdateVolume);
			BackButton.onClick.AddListener(() => {
				DiscardChanges();
				_mainMenuManager.ShowMain();

			});
			ApplySettingsButton.onClick.AddListener(ApplyChanges);
			gameObject.SetActive(true);
			UpdateView();
		}

		public void Hide() {
			VolumeSlider.onValueChanged.RemoveAllListeners();
			BackButton.onClick.RemoveAllListeners();
			ApplySettingsButton.onClick.RemoveAllListeners();
			gameObject.SetActive(false);
		}

		void UpdateVolume(float value) {
			AudioMixer.SetFloat(MasterVolume, value);
		}

		void UpdateView() {
			VolumeSlider.value = CurMasterVolume;
		}

		void DiscardChanges() {
			UpdateVolume(_settingsController.MasterVolume);
		}

		void ApplyChanges() {
			_settingsController.MasterVolume = CurMasterVolume;
		}
	}
}