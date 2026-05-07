using System;

namespace LSH.Core
{
    public static class SceneNameUtility
    {
        public static SceneReference From<TEnum>(TEnum scene)
            where TEnum : struct, Enum
        {
            return new SceneReference($"{Convert.ToInt32(scene):D2}_{scene}");
        }

        public static SceneReference From(Type enumType, string enumName)
        {
            if (enumType == null)
                throw new ArgumentNullException(nameof(enumType));

            if (!enumType.IsEnum)
                throw new ArgumentException($"{enumType.Name} is not an enum.");

            object value = Enum.Parse(enumType, enumName);
            int index = Convert.ToInt32(value);

            return new SceneReference($"{index:D2}_{enumName}");
        }
    }
}