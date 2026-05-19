using System.Collections.Generic;

namespace LSH.Core
{
    public interface ICoreBootstrapContext
    {
        IEnumerable<TimeChannelReference> TimeChannels { get; }
        CoreSceneSettings SceneSettings { get; }
    }
}