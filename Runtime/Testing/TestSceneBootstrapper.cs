using System;
using System.Collections.Generic;
using LSH.Core;
using UnityEngine;

namespace NewGame
{
    /// <summary>
    /// Entry Scene을 거치지 않고 현재 씬을 직접 실행할 때만
    /// 테스트용 Core Manager를 활성화하고 초기화합니다.
    /// </summary>
    [DefaultExecutionOrder(-10000)]
    [DisallowMultipleComponent]
    public sealed class TestSceneBootstrapper : MonoBehaviour
    {
        [Header("Test Core")]
        [Tooltip("InputManager, SoundManager 등이 들어 있는 비활성 GameObject")]
        [SerializeField] private GameObject _testCoreRoot;

        [Tooltip("ICoreBootstrapContext를 구현한 컴포넌트")]
        [SerializeField] private MonoBehaviour _contextObject;

        [Tooltip("초기화할 Core Manager 목록")]
        [SerializeField] private List<MonoBehaviour> _managerObjects = new();

        private void Awake()
        {
#if !UNITY_EDITOR
            // Editor 밖에서는 Development Build에서만 허용합니다.
            if (!Debug.isDebugBuild)
            {
                enabled = false;
                return;
            }
#endif

            if (HasExistingCore())
            {
                DisableTestCore();
                return;
            }

            if (!TryGetContext(out ICoreBootstrapContext context))
            {
                enabled = false;
                return;
            }

            if (_testCoreRoot == null)
            {
                Debug.LogError(
                    "Test Core Root가 할당되지 않았습니다.",
                    this);

                enabled = false;
                return;
            }

            if (_testCoreRoot.activeSelf)
            {
                Debug.LogError(
                    "Test Core Root는 Inspector에서 비활성화해 두어야 합니다.",
                    _testCoreRoot);

                enabled = false;
                return;
            }

            _testCoreRoot.SetActive(true);

            bool initializationFailed = !InitializeManagers(context);

            if (initializationFailed)
            {
                Debug.LogError(
                    "테스트용 Core Manager 초기화에 실패했습니다.",
                    this);
            }
            else
            {
                Debug.Log(
                    $"[TestSceneBootstrapper] {gameObject.scene.name} 직접 실행용 Core 초기화 완료.",
                    this);
            }

            enabled = false;
        }

        private bool InitializeManagers(ICoreBootstrapContext context)
        {
            if (_managerObjects == null)
            {
                Debug.LogError(
                    "Manager 목록이 할당되지 않았습니다.",
                    this);

                return false;
            }

            bool succeeded = true;

            foreach (MonoBehaviour managerObject in _managerObjects)
            {
                if (managerObject == null)
                {
                    Debug.LogError(
                        "Manager 목록에 Missing 참조가 있습니다.",
                        this);

                    succeeded = false;
                    continue;
                }

                try
                {
                    if (managerObject is IBootableWithContext contextBootable)
                    {
                        contextBootable.Init(context);
                    }
                    else if (managerObject is IBootable bootable)
                    {
                        bootable.Init();
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"{managerObject.name}은 지원되는 Bootable 인터페이스를 구현하지 않습니다.",
                            managerObject);
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, managerObject);
                    succeeded = false;
                }
            }

            return succeeded;
        }

        private bool TryGetContext(out ICoreBootstrapContext context)
        {
            context = _contextObject as ICoreBootstrapContext;

            if (context != null)
                return true;

            Debug.LogError(
                "Context가 없거나 ICoreBootstrapContext를 구현하지 않습니다.",
                this);

            return false;
        }

        private static bool HasExistingCore()
        {
            return FindAnyObjectByType<InputManager>() != null
                || FindAnyObjectByType<SoundManager>() != null
                || FindAnyObjectByType<TimeManager>() != null
                || FindAnyObjectByType<TransitionManager>() != null;
        }

        private void DisableTestCore()
        {
            if (_testCoreRoot != null)
                _testCoreRoot.SetActive(false);

            enabled = false;
        }
    }
}