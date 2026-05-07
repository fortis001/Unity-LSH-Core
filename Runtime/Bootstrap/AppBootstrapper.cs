using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace LSH.Core
{
    public class AppBootstrapper : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> _managerObjects;
        [SerializeField] private CoreSceneSettings _sceneSettings;
        [SerializeField] private TransitionManager _transitionManager;

        private IEnumerator Start()
        {
            _transitionManager.Configure(
                _sceneSettings.LoadingScene,
                _sceneSettings.FallbackScene);

            foreach (var obj in _managerObjects)
            {
                if (obj is IBootable manager)
                {
                    manager.Init();
                }
            }

            yield return new WaitForSeconds(0.2f);

            FinishInitialization();
        }

        private void FinishInitialization()
        {

            SceneReference targetScene = _sceneSettings.EntryCompleteScene;
            SceneManager.LoadScene(targetScene);
        }
    }
}

