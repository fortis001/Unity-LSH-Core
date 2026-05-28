namespace LSH.Core
{
    public class TimeChannel
    {
        public float Time { get; private set; }
        public float DeltaTime { get; private set; }
        public float FixedTime { get; private set; }
        public float FixedDeltaTime { get; private set; }
        public bool IsPaused { get; private set; }

        private bool _skipNextTick;
        private bool _skipNextFixedTick;

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

        public void FixedTick(float unscaledFixedDeltaTime)
        {
            if (_skipNextFixedTick)
            {
                FixedDeltaTime = 0f;
                _skipNextFixedTick = false;
                return;
            }

            if (IsPaused)
            {
                FixedDeltaTime = 0f;
                return;
            }

            FixedDeltaTime = unscaledFixedDeltaTime;
            FixedTime += FixedDeltaTime;
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
            _skipNextFixedTick = true;
        }

        public void Reset()
        {
            Time = 0f;
            DeltaTime = 0f;
            IsPaused = false;
            _skipNextTick = true;
            _skipNextFixedTick = true;
        }
    }
}