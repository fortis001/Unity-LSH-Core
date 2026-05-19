using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace LSH.Core
{
    public class AppBootstrapper : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> _managerObjects;
        [SerializeField] private MonoBehaviour _contextObject;

        private ICoreBootstrapContext _context;

        private IEnumerator Start()
        {
            if (!TryGetContext())
                yield break;

            foreach (var obj in _managerObjects)
            {
                if (obj is IBootableWithContext contextBootable)
                {
                    contextBootable.Init(_context);
                    continue;
                }

                if (obj is IBootable bootable)
                {
                    bootable.Init();
                }
            }

            yield return new WaitForSeconds(0.2f);

            FinishInitialization();
        }

        private bool TryGetContext()
        {
            _context = _contextObject as ICoreBootstrapContext;

            if (_context != null)
                return true;

            Debug.LogError(
                "Core bootstrap context is not assigned or does not implement ICoreBootstrapContext.",
                this);

            return false;
        }

        private void FinishInitialization()
        {
            SceneReference targetScene = _context.SceneSettings.EntryCompleteScene;
            SceneManager.LoadScene(targetScene);
        }
    }
}

