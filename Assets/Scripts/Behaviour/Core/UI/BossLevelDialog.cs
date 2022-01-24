using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Core.Enemy;
using STP.Behaviour.Sound;
using STP.Config;
using STP.Core;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using DG.Tweening;

namespace STP.Behaviour.Core.UI {
	public sealed class BossLevelDialog : GameComponent {
		[Header("Parameters")]
		public float StartDelay = 2f;
		public float       DialogBoxAppearDuration       = 0.5f;
		public List<float> DialogBoxPresentDurations     = new List<float>();
		public float       DialogBoxDisappearDuration    = 0.5f;
		public float       DialogBoxesInterval           = 2f;
		public float       DialogBoxVerticalMoveDuration = 0.2f;
		public float       BossStartAppearTime           = 10f;

		[Header("Dependencies")]
		[NotNull] public GameObject Root;
		[NotNull]        public Transform       HorizontalShowPos;
		[NotNull]        public Transform       HorizontalHidePos;
		[NotNullOrEmpty] public List<Transform> VerticalPositions = new List<Transform>();
		[NotNullOrEmpty] public List<Transform> DialogBoxes       = new List<Transform>();
		[Space]
		[NotNull] public AudioClip DialogBoxAppearSound;

		BaseBoss _boss;

		bool _needPlayAnim;

		Sequence _anim;

		void OnDisable() {
			_anim?.Kill();
		}

		void OnEnable() {
			if ( _needPlayAnim ) {
				PlayAnim();
				_needPlayAnim = false;
			}
		}

		public void Init(BaseBoss boss, LevelController levelController) {
			if ( levelController.CurLevelConfig.LevelType != LevelType.Boss ) {
				_needPlayAnim = false;
				Root.SetActive(false);
				return;
			}
			_boss         = boss;
			_needPlayAnim = true;
			Root.SetActive(true);

			_boss.PrepareAppear();

			var initialPos = new Vector3(HorizontalHidePos.localPosition.x, VerticalPositions[0].localPosition.y, 0);
			foreach ( var dialogBox in DialogBoxes ) {
				dialogBox.localPosition = initialPos;
			}
		}

		public void Deinit() {
			_anim?.Kill();
			_needPlayAnim = false;
		}

		void PlayAnim() {
			_anim = DOTween.Sequence().AppendInterval(StartDelay);
			for ( var i = 0; i < DialogBoxes.Count; i++ ) {
				var dialogBox = DialogBoxes[i];
				var time      = StartDelay + i * DialogBoxesInterval;
				var dialogBoxSeq = DOTween.Sequence()
					.AppendCallback(() => PersistentAudioPlayer.Instance.PlayOneShot(DialogBoxAppearSound))
					.Append(dialogBox.DOLocalMoveX(HorizontalShowPos.localPosition.x, DialogBoxAppearDuration))
					.AppendInterval(DialogBoxPresentDurations[i])
					.Append(dialogBox.DOLocalMoveX(HorizontalHidePos.localPosition.x, DialogBoxDisappearDuration))
					.SetUpdate(UpdateType.Manual);
				_anim.Insert(time, dialogBoxSeq);
				for ( var j = 0; j < i; ++j ) {
					var otherDialogBox = DialogBoxes[j];
					_anim.Insert(time, otherDialogBox.DOLocalMoveY(VerticalPositions[i + j].localPosition.y, DialogBoxVerticalMoveDuration));
				}
			}
			_anim.InsertCallback(BossStartAppearTime, _boss.Appear);
			_anim.OnComplete(() => {
					Root.SetActive(false);
					_anim = null;
				})
				.SetUpdate(UpdateType.Manual);
		}
	}
}
