using System;
using System.Collections.Generic;
using UnityEngine;

namespace LSH.Core
{
    public class TimeManager : Singleton<TimeManager>, IBootableWithContext
    {
        [SerializeField] private float _maxUnscaledDeltaTime = 0.1f;

        private readonly Dictionary<TimeChannelReference, TimeChannel> _channels = new();

        public event Action<TimeChannelReference> OnPaused;
        public event Action<TimeChannelReference> OnResumed;
        public event Action<TimeChannelReference> OnReset;

        private void Update()
        {
            float unscaledDeltaTime = Mathf.Min(
                Time.unscaledDeltaTime,
                _maxUnscaledDeltaTime);

            foreach (TimeChannel channel in _channels.Values)
            {
                channel.Tick(unscaledDeltaTime);
            }
        }

        public void Init(ICoreBootstrapContext context)
        {
            Init(context.TimeChannels);
        }

        private void Init(IEnumerable<TimeChannelReference> channelNames)
        {
            _channels.Clear();
            RegisterChannels(channelNames);
        }

        public void RegisterChannels(IEnumerable<TimeChannelReference> channelNames)
        {
            if (channelNames == null)
            {
                Debug.LogError("Time channel names are null.", this);
                return;
            }

            foreach (TimeChannelReference channelName in channelNames)
            {
                RegisterChannel(channelName);
            }
        }

        public void RegisterChannel(TimeChannelReference channelName)
        {
            if (channelName.IsEmpty)
            {
                Debug.LogError("TimeChannelReference is empty.", this);
                return;
            }

            if (_channels.ContainsKey(channelName))
                return;

            _channels.Add(channelName, new TimeChannel());
        }

        public TimeChannel GetChannel(TimeChannelReference channelName)
        {
            if (_channels.TryGetValue(channelName, out TimeChannel channel))
                return channel;

            Debug.LogError($"Time channel is not registered: {channelName}", this);
            return null;
        }

        public float GetTime(TimeChannelReference channelName)
        {
            TimeChannel channel = GetChannel(channelName);
            return channel != null ? channel.Time : 0f;
        }

        public float GetDeltaTime(TimeChannelReference channelName)
        {
            TimeChannel channel = GetChannel(channelName);
            return channel != null ? channel.DeltaTime : 0f;
        }

        public bool IsPaused(TimeChannelReference channelName)
        {
            TimeChannel channel = GetChannel(channelName);
            return channel != null && channel.IsPaused;
        }

        public void Pause(TimeChannelReference channelName)
        {
            TimeChannel channel = GetChannel(channelName);

            if (channel == null)
                return;

            if (channel.IsPaused)
                return;

            channel.Pause();
            OnPaused?.Invoke(channelName);
        }

        public void Resume(TimeChannelReference channelName)
        {
            TimeChannel channel = GetChannel(channelName);

            if (channel == null)
                return;

            if (!channel.IsPaused)
                return;

            channel.Resume();
            OnResumed?.Invoke(channelName);
        }

        public void Reset(TimeChannelReference channelName)
        {
            TimeChannel channel = GetChannel(channelName);

            if (channel == null)
                return;

            channel.Reset();
            OnReset?.Invoke(channelName);
        }

        public void ResetAndResume(TimeChannelReference channelName)
        {
            Reset(channelName);
            Resume(channelName);
        }
    }
}