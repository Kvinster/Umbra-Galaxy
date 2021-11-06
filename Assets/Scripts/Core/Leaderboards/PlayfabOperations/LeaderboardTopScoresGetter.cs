using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace STP.Core.Leaderboards {
	public class LeaderboardTopScoresGetter : BasePlayfabOperation {
		class OperationState {
			public List<PlayerLeaderboardEntry> Scores;
			public bool                         IsCompleted;
		}

		readonly string _leaderboardName;

		public LeaderboardTopScoresGetter(string leaderboardName) {
			_leaderboardName   = leaderboardName;
		}

		public async UniTask<List<PlayerLeaderboardEntry>> GetTopScores(int maxResultsCount) {
			if ( !IsLoggedIn ) {
				Debug.LogWarning("Trying to get leaderboard while not logged in");
				return new List<PlayerLeaderboardEntry>();
			}
			var state = new OperationState();
			PlayFabClientAPI.GetLeaderboard(FormRequest(maxResultsCount), (resultObj) => OnSuccessOperation(resultObj, state), (error) => OnError(error, state));
			await UniTask.WaitUntil(() => state.IsCompleted);
			return state.Scores;
		}


		void OnSuccessOperation(GetLeaderboardResult result, OperationState state) {
			state.Scores       = result.Leaderboard;
			state.IsCompleted  = true;
		}

		void OnError(PlayFabError error, OperationState state) {
			LogError(error);
			state.Scores      = null;
			state.IsCompleted = true;
		}
		
		GetLeaderboardRequest FormRequest(int maxResultsCount) {
			return new GetLeaderboardRequest {
				StatisticName   = _leaderboardName,
				MaxResultsCount = maxResultsCount
			};
		}
	}
}