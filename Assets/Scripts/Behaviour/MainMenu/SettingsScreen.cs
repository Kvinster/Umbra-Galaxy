using UnityEngine.Audio;
using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.Core;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.MainMenu {
	public class SettingsScreen : BaseMainMenuComponent {
		const string MasterVolumeId = SettingsController.MasterVolumeId;

		const float VolMin = -80;
		const float VolMax = 20;

		float CurMasterVolume {
			get {
				AudioMixer.GetFloat(MasterVolumeId, out var volume);
				return volume;
			}
		}

		[NotNull] public Slider     VolumeSlider;
		[NotNull] public AudioMixer AudioMixer;
		[NotNull] public Button     CancelButton;
		[NotNull] public Button     AcceptButton;

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
			CancelButton.onClick.AddListener(() => {
				DiscardChanges();
				_mainMenuManager.ShowMain();

			});
			AcceptButton.onClick.AddListener(() => {
				ApplyChanges();
				_mainMenuManager.ShowMain();
			});
			gameObject.SetActive(true);
			UpdateView();
		}

		public void Hide() {
			VolumeSlider.onValueChanged.RemoveAllListeners();
			CancelButton.onClick.RemoveAllListeners();
			AcceptButton.onClick.RemoveAllListeners();
			gameObject.SetActive(false);
		}

		void UpdateVolume(float value) {
			AudioMixer.SetFloat(MasterVolumeId, value);
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