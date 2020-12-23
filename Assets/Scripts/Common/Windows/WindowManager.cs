using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections.Generic;

using Object = UnityEngine.Object;

namespace STP.Common.Windows {
    public sealed class WindowManager {
        static WindowManager _instance;
        public static WindowManager Instance {
            get {
                TryCreate();
                return _instance;
            }
        }

        static readonly Dictionary<Type, string> TypeToPath = new Dictionary<Type, string> { };

        Canvas _uiCanvas;

        readonly Dictionary<Type, GameObject> _windowsCache  = new Dictionary<Type, GameObject>();
        readonly Stack<ActiveWindowId>        _activeWindows = new Stack<ActiveWindowId>();

        public event Action<ActiveWindowId> OnWindowShown;
        public event Action<Type>           OnWindowHidden;

        public bool HasActiveWindows => (_activeWindows.Count > 0);

        public void Show<T>(Action<T> init = null) where T : BaseWindow {
            var type = typeof(T);
            foreach ( var activeWindowId in _activeWindows ) {
                if ( activeWindowId.WindowType == type ) {
                    Debug.LogErrorFormat("Window '{0}' is already active", type.Name);
                    return;
                }
            }
            if ( !_windowsCache.TryGetValue(type, out var windowGo) ) {
                if ( !TypeToPath.TryGetValue(type, out var path) ) {
                    Debug.LogErrorFormat("Can't find path to window prefab for type '{0}'", type.Name);
                    return;
                }
                var windowPrefab = Resources.Load<GameObject>(path);
                if ( !windowPrefab ) {
                    Debug.LogErrorFormat("No window prefab in resources for type '{0}'", type.Name);
                    return;
                }
                windowGo = Object.Instantiate(windowPrefab, _uiCanvas.transform, false);
            }
            var window = windowGo.GetComponent<T>();
            if ( !window ) {
                Debug.LogErrorFormat("No '{0}' component on window root object", type.Name);
                return;
            }
            init?.Invoke(window);
            windowGo.transform.SetAsLastSibling();
            windowGo.gameObject.SetActive(true);
            ActiveWindowId id = (type, windowGo);
            _activeWindows.Push(id);
            OnWindowShown?.Invoke(id);
        }

        public void Hide(Type windowType) {
            var windowId = ActiveWindowId.Empty;
            foreach ( var activeWindowId in _activeWindows ) {
                if ( activeWindowId.WindowType == windowType ) {
                    windowId = activeWindowId;
                    break;
                }
            }
            if ( windowId.IsEmpty ) {
                Debug.LogErrorFormat("No '{0}' amongst active windows", windowType.Name);
                return;
            }
            var last = _activeWindows.Peek();
            if ( last != windowId ) {
                Debug.LogErrorFormat("Hiding non-top window '{0}'", windowType.Name);
                return;
            }
            _activeWindows.Pop();
            windowId.WindowRoot.SetActive(false);
            if ( !_windowsCache.ContainsKey(windowType) ) {
                _windowsCache.Add(windowType, windowId.WindowRoot);
            }
            OnWindowHidden?.Invoke(windowType);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
            UpdateCanvasCamera();
            HideAllWindows();
        }

        void UpdateCanvasCamera() {
            _uiCanvas.worldCamera = Camera.main;
        }

        void HideAllWindows() {
            while ( _activeWindows.Count > 0 ) {
                var window = _activeWindows.Peek();
                Hide(window.WindowType);
            }
        }

        WindowManager Init() {
            var canvasGo = new GameObject("[UICanvas]");
            Object.DontDestroyOnLoad(canvasGo);
            var rectTrans = canvasGo.AddComponent<RectTransform>();
            rectTrans.sizeDelta = new Vector2(1920, 1080);
            _uiCanvas = canvasGo.AddComponent<Canvas>();
            _uiCanvas.overrideSorting  = true;
            _uiCanvas.sortingLayerName = "UI";
            _uiCanvas.sortingOrder     = 1;
            _uiCanvas.renderMode       = RenderMode.ScreenSpaceCamera;
            _uiCanvas.planeDistance    = 50f;
            UpdateCanvasCamera();

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode            = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution    = new Vector2(1920, 1080);
            scaler.referencePixelsPerUnit = 100f;
            scaler.screenMatchMode        = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight     = 1f;

            canvasGo.AddComponent<GraphicRaycaster>();

            SceneManager.sceneLoaded += OnSceneLoaded;
            
            return this;
        }

        [RuntimeInitializeOnLoadMethod]
        static void TryCreate() {
            if ( _instance == null ) {
                _instance = new WindowManager().Init();
            }
        }
    }
}
