namespace LSH.Core
{
    public class TimeChannel
    {
        public float Time { get; private set; }
        public float DeltaTime { get; private set; }
        public bool IsPaused { get; private set; }

        private bool _skipNextTick;

        public void Tick(float unscaledDeltaTime)
        {
            if (_skipNextTick)
            {
                DeltaTime = 0f;
                _skipNextTick = false;
                return;
            }

            if (IsPaused)
            {
                DeltaTime = 0f;
                return;
            }

            DeltaTime = unscaledDeltaTime;
            Time += DeltaTime;
        }

        public void Pause()
        {
            IsPaused = true;
            DeltaTime = 0f;
        }

        public void Resume()
        {
            IsPaused = false;
            DeltaTime = 0f;
            _skipNextTick = true;
        }

        public void Reset()
        {
            Time = 0f;
            DeltaTime = 0f;
            IsPaused = false;
            _skipNextTick = true;
        }
    }
}