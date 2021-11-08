using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using STP.Core.Leaderboards.PlayfabOperations;
using UnityEngine;

namespace STP.Core.Leaderboards {
	public class LeaderboardController : BaseStateController  {
		const string LeaderBoardName = "Scores";

		const string SharedUserName = "Anonymous";
		
		string _displayName;
		
		public string PlayerId { get; private set; }
		
		bool IsLoggedIn => PlayFabClientAPI.IsClientLoggedIn();

		LeaderboardTopScoresGetter _topScoresGetter;
		LoginOperation             _loginOperation;
		
		public LeaderboardController() {
			_topScoresGetter = new LeaderboardTopScoresGetter(LeaderBoardName);
			_loginOperation  = new LoginOperation();
			UniTask.Create(() => TryLoginAsync());
		}
		
		public async UniTask PublishScoreAsync(int score) {
			if ( !IsLoggedIn ) {
				return;
			}
			var request            = FormPublishScoreRequest(score);
			var isPublishCompleted = false;
			PlayFabClientAPI.UpdatePlayerStatistics(request, _ => { isPublishCompleted = true; }, 
				(error) => HandleError(out isPublishCompleted, error));
			await UniTask.WaitWhile(() => !isPublishCompleted);
		}

		public async UniTask<List<Score>> GetTopScores(int recordsCount) {
			var playfabScores = await _topScoresGetter.GetTopScores(recordsCount);
			return ConvertPlayFabInfoToOurFormat(playfabScores);
		}

		public async UniTask<List<Score>> GetScoresAroundPlayerAsync(int recordsCount) {
			if ( !IsLoggedIn ) {
				return new List<Score>();
			}
			var isRequestCompleted = false;
			var request            = FormLeaderboardRequest(recordsCount);

			
			List<PlayerLeaderboardEntry> results = null;
			PlayFabClientAPI.GetLeaderboardAroundPlayer(request, (resultObj) => {
				results            = resultObj.Leaderboard;
				isRequestCompleted = true;
			}, (error) => HandleError(out isRequestCompleted, error));
			await UniTask.WaitWhile(() => !isRequestCompleted);
			return ConvertPlayFabInfoToOurFormat(results);
		}

		public async UniTask TryLoginAsync(bool isShared = true) {
			var (id, displayName) = (isShared) ? await _loginOperation.SharedLoginAsync() : await _loginOperation.UniqueLoginAsync();
			PlayerId = id;
			if ( string.IsNullOrEmpty(displayName) ) {
				await UpdateUserName(SharedUserName);
			}
		}

		public async UniTask UpdateUserName(string newName) {
			if ( string.IsNullOrEmpty(newName) ) {
				newName = SharedUserName;
			}
			var request     = FormUpdateUserNameRequest(newName);
			var isCompleted = false;
			PlayFabClientAPI.UpdateUserTitleDisplayName(request, _ => isCompleted = true,
				error => HandleError(out isCompleted, error));
			await UniTask.WaitUntil(() => isCompleted);
			Debug.Log($"name updated to {newName}");
		}

		List<Score> ConvertPlayFabInfoToOurFormat(List<PlayerLeaderboardEntry> scores) {
			return scores?
				.Where(score => !string.IsNullOrEmpty(score.DisplayName))
				.Select(score => new Score(score.Position + 1, score.StatValue, score.DisplayName, score.PlayFabId))
				.ToList();
		}

		void HandleError(out bool operationCompletionFlag, PlayFabError error) {
			Debug.LogError($"Playfab operation failed. Reason: {error.ErrorMessage}");
			operationCompletionFlag = true;
		} 

		UpdateUserTitleDisplayNameRequest FormUpdateUserNameRequest(string newName) {
			return new UpdateUserTitleDisplayNameRequest {
				DisplayName = newName
			};
		}
		
		GetLeaderboardAroundPlayerRequest FormLeaderboardRequest(int maxRecordsCount) {
			return new GetLeaderboardAroundPlayerRequest {
				StatisticName   = LeaderBoardName,
				MaxResultsCount = maxRecordsCount
			};
		}

		UpdatePlayerStatisticsRequest FormPublishScoreRequest(int score) {
			return new UpdatePlayerStatisticsRequest {
				Statistics = new List<StatisticUpdate> {
					new StatisticUpdate {
						StatisticName = LeaderBoardName,
						Value         = score
					}
				}
			};
		}
		
		
	}
}