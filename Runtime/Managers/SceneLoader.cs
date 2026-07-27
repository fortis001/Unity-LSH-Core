using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LSH.Core
{
    public class SceneLoader : MonoBehaviour
    {
        public static string TargetSceneName { get; set; }

        [SerializeField] private Slider _progressBar;
        [SerializeField] private TextMeshProUGUI _progressText;

        private AsyncOperation _operation;
        private bool _completionRaised;
        public static event Action<SceneLoader> OnLoadingCompleted;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            TargetSceneName = null;
            OnLoadingCompleted = null;
        }

        private void Start()
        {
            string targetSceneName = TargetSceneName;
            TargetSceneName = null;

            if (string.IsNullOrEmpty(targetSceneName))
            {
                TransitionManager transitionManager = TransitionManager.Instance;

                if (transitionManager == null)
                {
                    Debug.LogError("TransitionManager is not available.", this);
                    return;
                }

                SceneReference fallbackScene = transitionManager.FallbackScene;

                if (fallbackScene.IsEmpty)
                {
                    Debug.LogError("Fallback scene is empty.", this);
                    return;
                }

                try
                {
                    SceneManager.LoadScene(fallbackScene);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                }

                return;
            }

            StartCoroutine(LoadSceneAsync(targetSceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            try
            {
                _operation = SceneManager.LoadSceneAsync(sceneName);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }

            if (_operation == null)
            {
                Debug.LogError($"Failed to start loading scene: {sceneName}", this);
                yield break;
            }

            _operation.allowSceneActivation = false;

            float timer = 0f;
            float minLoadTime = 1.5f;

            while (!_operation.isDone)
            {
                float progress = Mathf.Clamp01(_operation.progress / 0.9f);

                timer += Time.unscaledDeltaTime;

                if (_progressBar != null)
                {
                    _progressBar.value = Mathf.Lerp(
                        _progressBar.value,
                        progress,
                        Time.unscaledDeltaTime * 5f);
                }

                if (_progressText != null)
                {
                    _progressText.text = $"{(int)(progress * 100)}%";
                }

                if (_operation.progress >= 0.9f && timer >= minLoadTime)
                {
                    RaiseLoadingCompleted();

                    yield break;
                }
                yield return null;
            }
        }

        public void ActivateScene()
        {
            if (_operation == null)
            {
                Debug.LogError("There is no pending scene load to activate.", this);
                return;
            }

            if (_progressBar != null)
            {
                _progressBar.value = 1.0f;
            }

            if (_progressText != null)
            {
                _progressText.text = "100%";
            }

            _operation.allowSceneActivation = true;
        }

        private void RaiseLoadingCompleted()
        {
            if (_completionRaised)
                return;

            _completionRaised = true;
            OnLoadingCompleted?.Invoke(this);
        }
    }
}

