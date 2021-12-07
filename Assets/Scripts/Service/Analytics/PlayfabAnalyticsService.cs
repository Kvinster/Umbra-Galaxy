using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace STP.Service {
    public class PlayfabAnalyticsService {
        public static bool CanLogEvents => PlayFabClientAPI.IsClientLoggedIn();
        
        public static void LogEvent<T>(T e) where T : struct, IAnalyticsEvent {
            if ( !CanLogEvents ) {
                return;
            }
            PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest {
                Body =  e.EventArgs,
                EventName =  e.EventName,
            }, OnLogEventFinished, OnLogEventFailed);
        }
        
        static void OnLogEventFailed(PlayFabError error) {
            Debug.LogError($"Can't send event. Reason {error}");
        }
        
        static void OnLogEventFinished(WriteEventResponse response) {
            Debug.Log($"Event sent. Result: {response.ToJson()}");
        }
    }
}