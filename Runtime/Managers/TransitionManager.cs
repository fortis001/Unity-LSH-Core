using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LSH.Core
{
    public class TransitionManager : Singleton<TransitionManager>, IBootableWithContext
    {

        [Header("UI References")]
        [SerializeField] private CanvasGroup _fadePanel;

        [Header("Settings")]
        [SerializeField] private float _fadeDuration = 0.5f;

        private SceneReference _loadingScene;
        private SceneReference _fallbackScene;

        private bool _isTransitioning = false;
        private bool _eventsSubscribed;
        private string _targetSceneName;
        private int _fadeVersion;

        public SceneReference FallbackScene => _fallbackScene;

        protected override void Awake()
        {
            base.Awake();
        }

        public void Init(ICoreBootstrapContext context)
        {
            if (context == null || context.SceneSettings == null)
            {
                Debug.LogError("Core scene settings are not available.", this);
                return;
            }

            _loadingScene = context.SceneSettings.LoadingScene;
            _fallbackScene = context.SceneSettings.FallbackScene;

            if (_loadingScene.IsEmpty)
                Debug.LogWarning("Loading scene is empty.", this);

            if (_fallbackScene.IsEmpty)
                Debug.LogWarning("Fallback scene is empty.", this);

            if (_fadePanel != null)
            {
                _fadePanel.alpha = 0f;
                _fadePanel.blocksRaycasts = false;
            }

            SubscribeEvents();
        }


        /// <summary>
        /// 씬 전환 통합 메서드
        /// </summary>
        /// <param name="targetScene">이동할 씬 이름</param>
        /// <param name="useLoadingScene">로딩 씬 사용 여부 (기본값: true)</param>
        public void LoadNextScene(SceneReference targetScene, bool useLoadingScene = true)
        {
            if (_isTransitioning) return;

            if (targetScene.IsEmpty)
            {
                Debug.LogError("Target scene is empty.", this);
                return;
            }

            if (useLoadingScene && _loadingScene.IsEmpty)
            {
                Debug.LogError("Loading scene is empty.", this);
                return;
            }

            _isTransitioning = true;
            _targetSceneName = targetScene.Value;
            SceneLoader.TargetSceneName = useLoadingScene ? targetScene.Value : null;

            if (useLoadingScene)
                StartCoroutine(SequenceWithLoading());
            else
                StartCoroutine(SequenceDirect());
        }

        #region Transition Sequences

        private IEnumerator SequenceWithLoading()
        {
            yield return StartCoroutine(Fade(1f));

            if (!TryLoadScene(_loadingScene))
            {
                CancelTransition();
                yield return StartCoroutine(Fade(0f));
            }
        }

        private IEnumerator SequenceDirect()
        {
            yield return StartCoroutine(Fade(1f));

            AsyncOperation operation = null;

            try
            {
                operation = SceneManager.LoadSceneAsync(_targetSceneName);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }

            if (operation == null)
            {
                CancelTransition();
                yield return StartCoroutine(Fade(0f));
                yield break;
            }

            while (!operation.isDone)
                yield return null;

        }

        private void HandleLoadingCompleted(SceneLoader loader)
        {
            if (!_isTransitioning || loader == null)
                return;

            StartCoroutine(SequenceToFinalTarget(loader));
        }

        private IEnumerator SequenceToFinalTarget(SceneLoader loader)
        {
            yield return StartCoroutine(Fade(1f));
            loader.ActivateScene();
        }

        #endregion

        #region Core Logic

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == _loadingScene || scene.name == _targetSceneName)
            {
                StartCoroutine(Fade(0f));
            }

            if (scene.name == _targetSceneName)
            {
                _isTransitioning = false;
            }
        }

        public IEnumerator Fade(float targetAlpha)
        {
            if (_fadePanel == null) yield break;

            int fadeVersion = ++_fadeVersion;

            _fadePanel.blocksRaycasts = true;

            if (_fadeDuration <= 0f)
            {
                _fadePanel.alpha = targetAlpha;
                _fadePanel.blocksRaycasts = targetAlpha > 0f;
                yield break;
            }

            float startAlpha = _fadePanel.alpha;
            float timer = 0f;

            while (timer < _fadeDuration && fadeVersion == _fadeVersion)
            {
                timer += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(timer / _fadeDuration);
                _fadePanel.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
                yield return null;
            }

            if (fadeVersion != _fadeVersion)
                yield break;

            _fadePanel.alpha = targetAlpha;

            if (targetAlpha <= 0f)
                _fadePanel.blocksRaycasts = false;
        }

        #endregion

        private void SubscribeEvents()
        {
            if (_eventsSubscribed)
                return;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneLoader.OnLoadingCompleted += HandleLoadingCompleted;
            _eventsSubscribed = true;
        }

        private void CancelTransition()
        {
            _isTransitioning = false;
            _targetSceneName = null;
            SceneLoader.TargetSceneName = null;
        }

        private bool TryLoadScene(SceneReference scene)
        {
            try
            {
                SceneManager.LoadScene(scene);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                return false;
            }
        }

        protected override void OnDestroy()
        {
            if (_eventsSubscribed)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                SceneLoader.OnLoadingCompleted -= HandleLoadingCompleted;
                _eventsSubscribed = false;
            }

            base.OnDestroy();
        }
    }
}
