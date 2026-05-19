using System;

namespace LSH.Core
{
    public static class TimeChannelNameUtility
    {
        public static TimeChannelReference From<TEnum>(TEnum channel)
            where TEnum : struct, Enum
        {
            Type enumType = typeof(TEnum);

            if (!Attribute.IsDefined(enumType, typeof(TimeChannelNameEnumAttribute)))
            {
                throw new ArgumentException(
                    $"{enumType.Name} must have [{nameof(TimeChannelNameEnumAttribute)}].");
            }

            return new TimeChannelReference(channel.ToString());
        }

        public static TimeChannelReference From(Type enumType, string enumName)
        {
            if (enumType == null)
                throw new ArgumentNullException(nameof(enumType));

            if (!enumType.IsEnum)
                throw new ArgumentException($"{enumType.Name} is not an enum.");

            if (!Attribute.IsDefined(enumType, typeof(TimeChannelNameEnumAttribute)))
            {
                throw new ArgumentException(
                    $"{enumType.Name} must have [{nameof(TimeChannelNameEnumAttribute)}].");
            }

            object value = Enum.Parse(enumType, enumName);
            return new TimeChannelReference(value.ToString());
        }
    }
}