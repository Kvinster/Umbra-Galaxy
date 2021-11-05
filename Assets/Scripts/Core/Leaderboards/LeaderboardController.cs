using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace STP.Core.Leaderboards {
	public class LeaderboardController : BaseStateController  {
		const string LeaderBoardName = "Scores";
		
		string _playfabId;
		string _displayName;
		
		bool IsLoggedIn => PlayFabClientAPI.IsClientLoggedIn();

		public LeaderboardController() {
			TryLoginAsync();
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

		public async UniTask TryLoginAsync() {
			if ( IsLoggedIn ) {
				Debug.LogWarning("You are already logged in.");
				return;
			}
			var request            = FormLoginRequest();
			var isRequestCompleted = false;
			PlayFabClientAPI.LoginWithCustomID(request, (result) => {
				isRequestCompleted = true;
				_displayName       = result.InfoResultPayload.PlayerProfile?.DisplayName ?? "Anonymous";
				_playfabId         = result.PlayFabId;
			}, (error) => HandleError(out isRequestCompleted, error));
			await UniTask.WaitWhile(() => !isRequestCompleted);
		}

		public async UniTaskVoid UpdateUserName(string newName) {
			var request     = FormUpdateUserNameRequest(newName);
			var isCompleted = false;
			PlayFabClientAPI.UpdateUserTitleDisplayName(request, _ => isCompleted = true,
				error => HandleError(out isCompleted, error));
			await UniTask.WaitUntil(() => isCompleted);
		}

		List<Score> ConvertPlayFabInfoToOurFormat(List<PlayerLeaderboardEntry> scores) {
			return scores?.Where(score => !string.IsNullOrEmpty(score.DisplayName)).Select(score => new Score(score.Position, score.StatValue, score.DisplayName)).ToList();
		}

		void HandleError(out bool operationCompletionFlag, PlayFabError error) {
			Debug.LogError($"Playfab operation failed. Reason: {error.ErrorMessage}");
			operationCompletionFlag = true;
		} 

		LoginWithCustomIDRequest FormLoginRequest() {
			return new LoginWithCustomIDRequest{
				CustomId      = SystemInfo.deviceUniqueIdentifier,
				CreateAccount = true,
				InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
					GetPlayerProfile = true
				}
			};
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