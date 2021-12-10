using PlayFab;
using PlayFab.ClientModels;
using STP.Manager;
using UnityEngine;

namespace STP.Service {
    public class AnalyticsService {
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

        [RuntimeInitializeOnLoadMethod]
        static void StartTraceExceptions() {
            Application.logMessageReceived += TraceExceptionToTheAnalytics;
        }

        static void TraceExceptionToTheAnalytics(string condition, string stackTrace, LogType logType) {
            if ( logType == LogType.Exception ) {
                LogEvent(new UnhandledExceptionEvent(condition, stackTrace));
            }
        }
        
        static void OnLogEventFailed(PlayFabError error) {
            Debug.LogError($"Can't send event. Reason {error}");
        }
        
        static void OnLogEventFinished(WriteEventResponse response) {
            Debug.Log($"Event sent. Result: {response.ToJson()}");
        }
    }
}