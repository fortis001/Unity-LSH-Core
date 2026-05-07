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
        public static event Action<SceneLoader> OnLoadingCompleted;


        private void Start()
        {
            if (string.IsNullOrEmpty(TargetSceneName))
            {
                SceneManager.LoadScene(SceneName.Title);
                return;
            }

            StartCoroutine(LoadSceneAsync(TargetSceneName));

        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            _operation = SceneManager.LoadSceneAsync(sceneName);
            _operation.allowSceneActivation = false;

            float timer = 0f;
            float minLoadTime = 1.5f;

            while (!_operation.isDone)
            {
                float progress = Mathf.Clamp01(_operation.progress / 0.9f);

                timer += Time.deltaTime;

                if (_progressBar != null)
                {
                    _progressBar.value = Mathf.Lerp(_progressBar.value, progress, Time.deltaTime * 5f);
                }

                if (_progressText != null)
                {
                    _progressText.text = $"{(int)(progress * 100)}%";
                }

                if (_operation.progress >= 0.9f && timer >= minLoadTime)
                {

                    OnLoadingCompleted?.Invoke(this);

                    yield break;
                }
                yield return null;
            }
        }

        public void ActivateScene()
        {
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
    }
}

