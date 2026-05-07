using System;

namespace LSH.Core
{
    [AttributeUsage(AttributeTargets.Enum)]
    public sealed class SoundIdEnumAttribute : Attribute
    {
        public SoundType Type { get; }

        public SoundIdEnumAttribute(SoundType type)
        {
            Type = type;
        }
    }
}