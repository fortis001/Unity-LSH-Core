using System;
using UnityEngine;

namespace LSH.Core
{
    [Serializable]
    public struct SoundId
    {
        [SerializeField] private int _id;
        [SerializeField] private string _name;

        public int Id => _id;
        public string Name => _name;

        public bool IsEmpty => _id == 0;

        private SoundId(int id, string name)
        {
            _id = id;
            _name = name;
        }

        public static SoundId From<TEnum>(TEnum value)
            where TEnum : struct, Enum
        {
            Type enumType = typeof(TEnum);

            SoundIdEnumAttribute attribute =
                Attribute.GetCustomAttribute(enumType, typeof(SoundIdEnumAttribute))
                as SoundIdEnumAttribute;

            if (attribute == null)
            {
                throw new ArgumentException(
                    $"{enumType.Name} must have [{nameof(SoundIdEnumAttribute)}].");
            }

            int id = Convert.ToInt32(value);
            string name = value.ToString();

            return new SoundId(id, name);
        }

        public static implicit operator int(SoundId soundId)
        {
            return soundId._id;
        }

        public override string ToString()
        {
            return $"{_id} : {_name}";
        }
    }
}