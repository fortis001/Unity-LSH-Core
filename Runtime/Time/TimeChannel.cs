

namespace LSH.Core
{
    public class TimeChannel
    {
        public float Time { get; private set; }
        public float DeltaTime { get; private set; }
        public bool IsPaused { get; private set; }

        public void Tick(float unscaledDeltaTime)
        {
            DeltaTime = IsPaused ? 0f : unscaledDeltaTime;
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
        }

        public void Reset()
        {
            Time = 0f;
            DeltaTime = 0f;
            IsPaused = false;
        }
    }
}