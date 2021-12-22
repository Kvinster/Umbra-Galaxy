using UnityEngine;

using System.Collections.Generic;

using STP.Config;
using STP.Core;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using DG.Tweening;

namespace STP.Behaviour.Core.UI {
	public sealed class BossLevelDialog : GameComponent {
		[Header("Parameters")]
		public float StartDelay = 2f;
		public float DialogBoxAppearDuration       = 0.5f;
		public float DialogBoxPresentDuration      = 3f;
		public float DialogBoxDisappearDuration    = 0.5f;
		public float DialogBoxesInterval           = 2f;
		public float DialogBoxVerticalMoveDuration = 0.2f;

		[Header("Dependencies")]
		[NotNull] public GameObject Root;
		[NotNull]        public Transform       HorizontalShowPos;
		[NotNull]        public Transform       HorizontalHidePos;
		[NotNullOrEmpty] public List<Transform> VerticalPositions = new List<Transform>();
		[NotNullOrEmpty] public List<Transform> DialogBoxes       = new List<Transform>();

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

		public void Init(LevelController levelController) {
			if ( levelController.CurLevelConfig.LevelType != LevelType.Boss ) {
				_needPlayAnim = false;
				Root.SetActive(false);
				return;
			}
			_needPlayAnim = true;
			Root.SetActive(true);

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
					.Append(dialogBox.DOLocalMoveX(HorizontalShowPos.localPosition.x, DialogBoxAppearDuration))
					.AppendInterval(DialogBoxPresentDuration)
					.Append(dialogBox.DOLocalMoveX(HorizontalHidePos.localPosition.x, DialogBoxDisappearDuration));
				_anim.Insert(time, dialogBoxSeq);
				for ( var j = 0; j < i; ++j ) {
					var otherDialogBox = DialogBoxes[j];
					_anim.Insert(time, otherDialogBox.DOLocalMoveY(VerticalPositions[i + j].localPosition.y, DialogBoxVerticalMoveDuration));
				}
			}
			_anim.OnComplete(() => {
				Root.SetActive(false);
				_anim = null;
			});
		}
	}
}
