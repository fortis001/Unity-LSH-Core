using System;
using UnityEngine;

namespace LSH.Core
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SoundIdFieldAttribute : PropertyAttribute
    {
        public SoundType Type { get; }

        public SoundIdFieldAttribute(SoundType type)
        {
            Type = type;
        }
    }
}
