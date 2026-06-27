using System;
using UnityEngine;

namespace LSH.Core
{
    [Serializable]
    public struct TimeChannelReference : IEquatable<TimeChannelReference>
    {
        [SerializeField] private string _value;

        public string Value => _value;
        public bool IsEmpty => string.IsNullOrWhiteSpace(_value);

        public TimeChannelReference(string value)
        {
            _value = value;
        }

        public bool Equals(TimeChannelReference other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is TimeChannelReference other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value != null ? _value.GetHashCode() : 0;
        }

        public static bool operator ==(
            TimeChannelReference left,
            TimeChannelReference right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(
            TimeChannelReference left,
            TimeChannelReference right)
        {
            return !left.Equals(right);
        }

        public static implicit operator string(TimeChannelReference channel)
        {
            return channel._value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}