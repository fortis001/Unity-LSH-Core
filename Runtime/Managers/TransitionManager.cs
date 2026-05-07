using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LSH.Core
{
    public class TransitionManager : Singleton<TransitionManager>, IBootable
    {

        [Header("UI References")]
        [SerializeField] private CanvasGroup _fadePanel;

        [Header("Settings")]
        [SerializeField] private float _fadeDuration = 0.5f;

        private SceneReference _loadingScene;
        private SceneReference _fallbackScene;

        private bool _isTransitioning = false;
        private string _targetSceneName;
        private Coroutine _fadeCoroutine;

        public SceneReference FallbackScene => _fallbackScene;

        protected override void Awake()
        {
            base.Awake();
        }

        public void Configure(SceneReference loadingScene, SceneReference fallbackScene)
        {
            _loadingScene = loadingScene;
            _fallbackScene = fallbackScene;
        }

        public void Init()
        {
            if (_fadePanel != null)
            {
                _fadePanel.alpha = 0f;
                _fadePanel.blocksRaycasts = false;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneLoader.OnLoadingCompleted += HandleLoadingCompleted;
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

            _isTransitioning = true;
            _targetSceneName = targetScene.Value;
            SceneLoader.TargetSceneName = targetScene.Value;

            if (useLoadingScene)
                StartCoroutine(SequenceWithLoading());
            else
                StartCoroutine(SequenceDirect());
        }

        #region Transition Sequences

        private IEnumerator SequenceWithLoading()
        {
            yield return StartCoroutine(Fade(1f));

            SceneManager.LoadScene(_loadingScene);
        }

        private IEnumerator SequenceDirect()
        {
            yield return StartCoroutine(Fade(1f));

            var op = SceneManager.LoadSceneAsync(_targetSceneName);
            while (!op.isDone) yield return null;

        }

        private void HandleLoadingCompleted(SceneLoader loader)
        {
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

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            _fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));

            yield return _fadeCoroutine;

            _fadeCoroutine = null;
        }

        private IEnumerator FadeRoutine(float targetAlpha)
        {
            _fadePanel.blocksRaycasts = true;
            float startAlpha = _fadePanel.alpha;
            float timer = 0f;

            while (timer < _fadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                _fadePanel.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / _fadeDuration);
                yield return null;
            }

            _fadePanel.alpha = targetAlpha;

            if (targetAlpha <= 0f)
                _fadePanel.blocksRaycasts = false;
        }

        #endregion

        protected override void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneLoader.OnLoadingCompleted -= HandleLoadingCompleted;

            base.OnDestroy();
        }
    }
}