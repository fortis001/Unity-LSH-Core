using System;
using UnityEngine;

namespace LSH.Core
{
    [Serializable]
    public struct SceneReference
    {
        [SerializeField] private string _value;

        public string Value => _value;
        public bool IsEmpty => string.IsNullOrWhiteSpace(_value);

        public SceneReference(string value)
        {
            _value = value;
        }

        public static implicit operator string(SceneReference scene)
        {
            return scene._value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
