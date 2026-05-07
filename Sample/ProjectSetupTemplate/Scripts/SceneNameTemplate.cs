using LSH.Core;

namespace NewGame
{
    [SceneNameEnum]
    public enum SceneNameType
    {
        Entry = 0,
        Title = 1,
        InGame = 2,
        Result = 3,

        Loading = 99
    }

    public static class SceneName
    {
        public static SceneReference Entry => SceneNameUtility.From(SceneNameType.Entry);
        public static SceneReference Title => SceneNameUtility.From(SceneNameType.Title);
        public static SceneReference InGame => SceneNameUtility.From(SceneNameType.InGame);
        public static SceneReference Result => SceneNameUtility.From(SceneNameType.Result);

        public static SceneReference Loading => SceneNameUtility.From(SceneNameType.Loading);
    }
}