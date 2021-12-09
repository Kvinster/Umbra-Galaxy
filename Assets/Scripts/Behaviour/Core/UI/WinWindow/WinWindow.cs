using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using STP.Core;
using STP.Core.Leaderboards;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using Cysharp.Threading.Tasks;
using DG.Tweening;
using IPromise = RSG.IPromise;

namespace STP.Behaviour.Core.UI.WinWindow {
	public sealed class WinWindow : BaseCoreWindow {
		[NotNull] public Transform   Title;
		[NotNull] public CanvasGroup TitleCanvasGroup;
		
		[NotNull] public List<LeaderboardEntryView> Entries;
		[NotNull] public GameObject                 EntriesRoot;

		[NotNull] public Button ContinueButton;

		[NotNull] public GameObject  LoadingRoot;
		[NotNull] public CanvasGroup LoadingCanvasGroup;
		
		[NotNull] public CanvasGroup LeaderboardRootCanvasGroup;

		LeaderboardController _leaderboardController;
		LevelManager          _levelManager;
		ScoreController       _scoreController;

		List<Score> _scores;

		LeaderboardEntryView _activePlayerView;

		Sequence _activeSequence;
		
		public void CommonInit(ScoreController scoreController, LeaderboardController leaderboardController, LevelManager levelManager) {
			_scoreController       = scoreController;
			_leaderboardController = leaderboardController;
			_levelManager          = levelManager;
			ContinueButton.onClick.AddListener(OnContinueClick);
		}

		public override IPromise Show() {
			var promise = base.Show();

			_activeSequence = CreateLoadingAnimation().AppendCallback(() => _activeSequence = null);
			
			LoadingRoot.gameObject.SetActive(true);
			EntriesRoot.SetActive(false);
			UpdateLeaderboard().Forget();
			return promise;
		}

		Sequence CreateLoadingAnimation() {
			var endPos = Title.position;

			Title.position           = Vector3.zero;
			TitleCanvasGroup.alpha   = 0f;
			LoadingCanvasGroup.alpha = 0f;

			var seq = DOTween.Sequence()
				.Append(TitleCanvasGroup.DOFade(1f, 0.3f))
				.Append(Title.DOMove(endPos, 0.3f))
				.Append(LoadingCanvasGroup.DOFade(1f, 0.3f))
				.SetUpdate(true);
			return seq;
		}

		Sequence CreateShowLeaderboardAnimation() {
			LeaderboardRootCanvasGroup.alpha = 0f;
			
			var seq = DOTween.Sequence()
				.Append(LoadingCanvasGroup.DOFade(0f, 0.3f))
				.AppendCallback(() => {
					LoadingCanvasGroup.gameObject.SetActive(false);
					LeaderboardRootCanvasGroup.gameObject.SetActive(true);
					InitViews();
				})
				.Append(LeaderboardRootCanvasGroup.DOFade(1f, 0.3f))
				.SetUpdate(true);
			return seq;
		}
		
		void InitViews() {
			if ( _scores == null ) {
				return;
			}
			// reset all views
			if ( _activePlayerView ) {
				_activePlayerView.OnEndNameEdition -= FinishUserNameInput;
				_activePlayerView                  =  null;
			}
			foreach ( var entry in Entries ) {
				entry.Reset();
			}
			// show all scores
			var viewIndex = 0;
			foreach ( var score in _scores ) {
				var view = Entries[viewIndex];
				view.ShowEntry(score);
				if ( score.Id == _leaderboardController.PlayerId ) {
					_activePlayerView = view;
					_activePlayerView.SetAsCurrentPlayerView();
					_activePlayerView.OnEndNameEdition += FinishUserNameInput;
				}
				viewIndex++;
			}
			
			// hide unnecessary views
			for ( var i = viewIndex; i < Entries.Count; i++) {
				Entries[i].Hide();
			}
		}

		void SelectActivePlayerInputField() {
			if ( !_activePlayerView ) {
				return;
			}
			_activePlayerView.PlayerNameText.Select();
		}
		
		void FinishUserNameInput(string lastResult) {
			UniTask.Create(() => _leaderboardController.UpdateUserName(lastResult));
		}

		async UniTaskVoid UpdateLeaderboard() {
			await _leaderboardController.TryLoginAsync();
			await _leaderboardController.PublishScoreAsync(_scoreController.Score);
			await UniTask.Delay(1000, DelayType.UnscaledDeltaTime);
			_scores             = await _leaderboardController.GetScoresAroundPlayerAsync(Entries.Count);
			if ( _activeSequence == null ) {
				_activeSequence = CreateShowLeaderboardAnimation();
			} else {
				_activeSequence.Append(CreateShowLeaderboardAnimation());
			}
		}

		void OnContinueClick() {
			Hide();
			_levelManager.QuitToMenu();
		}
	}
}
