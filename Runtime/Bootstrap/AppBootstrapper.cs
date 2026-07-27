using System;
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

        private void Start()
        {
            if (!TryGetContext())
                return;

            if (_managerObjects == null)
            {
                Debug.LogError("Core manager list is not assigned.", this);
                return;
            }

            bool initializationFailed = false;

            foreach (MonoBehaviour obj in _managerObjects)
            {
                if (obj == null)
                {
                    Debug.LogError("Core manager list contains a missing reference.", this);
                    initializationFailed = true;
                    continue;
                }

                try
                {
                    if (obj is IBootableWithContext contextBootable)
                    {
                        contextBootable.Init(_context);
                        continue;
                    }

                    if (obj is IBootable bootable)
                    {
                        bootable.Init();
                        continue;
                    }

                    Debug.LogWarning(
                        $"{obj.name} does not implement a supported bootable interface.",
                        obj);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, obj);
                    initializationFailed = true;
                }
            }

            if (initializationFailed)
            {
                Debug.LogError(
                    "Core initialization failed. Entry scene loading was cancelled.",
                    this);
                return;
            }

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
            CoreSceneSettings sceneSettings = _context.SceneSettings;

            if (sceneSettings == null)
            {
                Debug.LogError("Core scene settings are not assigned.", this);
                return;
            }

            SceneReference targetScene = sceneSettings.EntryCompleteScene;

            if (targetScene.IsEmpty)
            {
                Debug.LogError("Entry complete scene is empty.", sceneSettings);
                return;
            }

            SceneManager.LoadScene(targetScene);
        }
    }
}

