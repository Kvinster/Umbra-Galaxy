﻿using UnityEngine;
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
		[NotNull] public List<LeaderboardEntryView> Entries;
		[NotNull] public GameObject                 EntriesRoot;

		[NotNull] public Button ContinueButton;

		[NotNull] public GameObject LoadingRoot;

		LeaderboardController _leaderboardController;
		LevelManager          _levelManager;
		ScoreController       _scoreController;

		List<Score> _scores;

		LeaderboardEntryView _activePlayerView;

		public void CommonInit(ScoreController scoreController, LeaderboardController leaderboardController, LevelManager levelManager) {
			_scoreController       = scoreController;
			_leaderboardController = leaderboardController;
			_levelManager          = levelManager;
			ContinueButton.onClick.AddListener(OnContinueClick);
		}

		public override IPromise Show() {
			var promise = base.Show();
			LoadingRoot.gameObject.SetActive(true);
			EntriesRoot.SetActive(false);
			UpdateLeaderboard().Forget();
			return promise;
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
			_scores = await _leaderboardController.GetScoresAroundPlayerAsync(Entries.Count);
			LoadingRoot.gameObject.SetActive(false);
			EntriesRoot.SetActive(true);
			InitViews();
		}

		void OnContinueClick() {
			Hide();
			_levelManager.QuitToMenu();
		}
	}
}
