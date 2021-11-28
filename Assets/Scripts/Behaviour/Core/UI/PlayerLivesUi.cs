using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;

using STP.Core;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using DG.Tweening;

using Random = UnityEngine.Random;

namespace STP.Behaviour.Core.UI {
	public sealed class PlayerLivesUi : GameComponent {
		[Header("Parameters")]
		[Header("Start params")]
		public Color NormalColor;
		[Header("Lose live anim")]
		public float ShakeMagnitude;
		public float ShakeDuration;
		public float UpscaleAmount;
		public float UpscaleDuration;
		public Color DisappearColor;
		public float DisappearColorDuration;
		[Header("Gain live anim")]
		public float AppearDuration;
		[Header("Dependencies")]
		[NotNull]
		public GameObject Root;
		[NotNull]
		public HorizontalLayoutGroup Layout;
		[NotNullOrEmpty]
		public List<Image> Elements;

		PlayerController _playerController;

		Tween _appearAnim;

		void OnDisable() {
			if ( _playerController != null ) {
				_playerController.OnLifeAdded -= OnPlayerGainLive;
			}
			_appearAnim?.Kill();
		}

		public void Init(PlayerController playerController) {
			_playerController = playerController;

			for ( var i = 0; i < Elements.Count; i++ ) {
				var element = Elements[i];
				element.color                = NormalColor;
				element.transform.localScale = Vector3.one;
				element.gameObject.SetActive(i < playerController.CurLives - 1);
			}

			_playerController.OnLifeAdded += OnPlayerGainLive;
		}

		void Update() {
			if ( Input.GetKeyDown(KeyCode.Q) ) {
				OnPlayerGainLive();
			}
		}

		[NaughtyAttributes.Button("Test")]
		public Tween PlayLoseLiveAnim() {
			Image element = null;
			for ( var i = Elements.Count - 1; i >= 0; i-- ) {
				var tmpElement = Elements[i];
				if ( tmpElement.gameObject.activeInHierarchy ) {
					element = tmpElement;
					break;
				}
			}
			if ( !element ) {
				Debug.LogErrorFormat("LivesUi.PlayLoseLiveAnim: can't find active element");
				return null;
			}
			var seq                = DOTween.Sequence();
			var shakeProgress      = 0f;
			var elementOriginalPos = element.transform.localPosition;
			seq.Append(DOTween.To(() => shakeProgress, x => {
					shakeProgress = x;
					element.transform.localPosition = elementOriginalPos + (Vector3) Random.insideUnitCircle *
						(Mathf.Sin(shakeProgress * Mathf.PI) * ShakeMagnitude);
				}, 1f, ShakeDuration))
				.AppendCallback(() => element.transform.localPosition = elementOriginalPos)
				.Append(element.transform.DOScale(Vector3.one * UpscaleAmount, UpscaleDuration))
				.Join(element.DOColor(DisappearColor, DisappearColorDuration))
				.OnComplete(() => element.gameObject.SetActive(false))
				.SetUpdate(true);
			return seq;
		}

		void OnPlayerGainLive() {
			Image element = null;
			for ( var i = 0; i < Elements.Count; ++i ) {
				var tmpElement = Elements[i];
				if ( !tmpElement.gameObject.activeInHierarchy ) {
					element = tmpElement;
					break;
				}
			}
			if ( !element ) {
				Debug.LogErrorFormat("LivesUi.PlayLoseLiveAnim: can't find inactive element");
				return;
			}
			element.color = new Color(NormalColor.r, NormalColor.g, NormalColor.b, 0f);
			_appearAnim?.Kill(true);
			_appearAnim = DOTween.Sequence()
				.AppendCallback(() => element.gameObject.SetActive(true))
				.Append(element.DOColor(NormalColor, AppearDuration))
				.OnComplete(() => _appearAnim = null)
				.SetUpdate(true);
		}
	}
}
