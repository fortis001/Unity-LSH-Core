using UnityEngine;

namespace LSH.Core
{
    [CreateAssetMenu(
        fileName = "CoreSceneSettings",
        menuName = "LSH/Core/Scene Settings")]
    public class CoreSceneSettings : ScriptableObject
    {
        [SerializeField] private string _sceneEnumTypeName;

        [SerializeField] private SceneReference _entryCompleteScene;
        [SerializeField] private SceneReference _loadingScene;
        [SerializeField] private SceneReference _fallbackScene;

        public string SceneEnumTypeName => _sceneEnumTypeName;

        public SceneReference EntryCompleteScene => _entryCompleteScene;
        public SceneReference LoadingScene => _loadingScene;
        public SceneReference FallbackScene => _fallbackScene;
    }
}