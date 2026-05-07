using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace LSH.Core
{
    public class AppInitializer : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> _managerObjects;

        private IEnumerator Start()
        {
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
            SceneManager.LoadScene(SceneName.Title);
        }
    }
}

