using System;
using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Internal;
using UnityEngine;


namespace STP.Core.Leaderboards.PlayfabOperations {
	public class LoginOperation : BasePlayfabOperation {
		class OperationState {
			public bool   IsCompleted;
			public string PlayerId;
			public string DisplayName;
		}
		
		const string SharedUserId = "__Anonymous__";

		public UniTask<(string id, string displayName)> SharedLoginAsync() => Login(SharedUserId);

		public UniTask<(string id, string displayName)> UniqueLoginAsync() => Login(Guid.NewGuid().ToString());

		async UniTask<(string id, string displayName)> Login(string userId) {
			var state = new OperationState();
			PlayFabClientAPI.LoginWithCustomID(
				FormLoginRequest(userId), 
				(result) => OnLoginSuccess(result, state), 
				(error) => OnLoginFailed(error, state));
			await UniTask.WaitUntil(() => state.IsCompleted);
			return (state.PlayerId, state.DisplayName);
		}

		void OnLoginSuccess(LoginResult result, OperationState state) {
			state.IsCompleted = true;
			state.PlayerId    = result.PlayFabId;
			state.DisplayName = result.InfoResultPayload.PlayerProfile?.DisplayName;
			Debug.Log($"Login completed. Id: {result.PlayFabId}");
		}

		void OnLoginFailed(PlayFabError error, OperationState state) {
			LogError(error);
			state.IsCompleted = true;
		}

		LoginWithCustomIDRequest FormLoginRequest(string userId) {
			return new LoginWithCustomIDRequest{
				CustomId      = userId,
				CreateAccount = true,
				InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
					GetPlayerProfile = true
				}
			};
		}
	}
}