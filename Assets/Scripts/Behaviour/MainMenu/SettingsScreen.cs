using UnityEngine.Audio;
using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.MainMenu {
	public class SettingsScreen : BaseMainMenuComponent {
		const string MasterVolume = "masterVol";

		const float VolMin = -80;
		const float VolMax = 20;

		[NotNull] public Slider     VolumeSlider;
		[NotNull] public AudioMixer AudioMixer;
		[NotNull] public Button     BackButton;

		MainMenuManager _mainMenuManager;

		protected override void InitInternal(MainMenuStarter starter) {
			VolumeSlider.minValue = VolMin;
			VolumeSlider.maxValue = VolMax;
			_mainMenuManager      = starter.MainMenuManager;
		}

		public void Show() {
			VolumeSlider.onValueChanged.AddListener(UpdateVolume);
			BackButton.onClick.AddListener(_mainMenuManager.ShowMain);

			gameObject.SetActive(true);
			UpdateView();
		}

		public void Hide() {
			VolumeSlider.onValueChanged.RemoveAllListeners();
			BackButton.onClick.RemoveAllListeners();
			gameObject.SetActive(false);
		}

		void UpdateVolume(float value) {
			AudioMixer.SetFloat(MasterVolume, value);
			print("Volume" + value);
		}

		void UpdateView() {
			AudioMixer.GetFloat(MasterVolume, out var volume);
			VolumeSlider.value = volume;
		}

	}
}