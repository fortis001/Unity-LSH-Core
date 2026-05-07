namespace LSH.Core
{
    public enum SceneNameType
    {
        Entry,
        Title,
        InGame,
        Result,



        Loading = 99
    }

    public struct SceneName
    {
        private readonly string _value;

        private SceneName(SceneNameType type)
        {
            _value = $"{(int)type:D2}_{type}";
        }

        public static implicit operator string(SceneName sceneName) => sceneName._value;



        public static SceneName Entry => new SceneName(SceneNameType.Entry);
        public static SceneName Title => new SceneName(SceneNameType.Title);
        public static SceneName InGame => new SceneName(SceneNameType.InGame);
        public static SceneName Result => new SceneName(SceneNameType.Result);



        public static SceneName Loading => new SceneName(SceneNameType.Loading);

        public override string ToString() => _value;
    }
}
