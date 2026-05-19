using UnityEngine;

namespace NewGame
{
    public class BootstrapContextTemplate : MonoBehaviour, ICoreBootstrapContext
    {
        [SerializeField] private CoreSceneSettings _sceneSettings;

        public IEnumerable<TimeChannelReference> TimeChannels =>
                TimeChannelName.All;

        public CoreSceneSettings SceneSettings => _sceneSettings;
    }
}

