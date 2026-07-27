#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LSH.Core.Editor
{
    [CustomPropertyDrawer(typeof(SoundId))]
    public class SoundIdDrawer : PropertyDrawer
    {
        private sealed class SoundIdDropdownCache
        {
            public Type EnumType;
            public string[] Names;
            public int[] Ids;
            public string[] DisplayOptions;
            public string Error;
        }

        private static readonly Dictionary<SoundType, SoundIdDropdownCache> _cacheByType = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty idProperty = property.FindPropertyRelative("_id");
            SerializedProperty nameProperty = property.FindPropertyRelative("_name");

            SoundType? type = GetSoundKind();

            if (type == null)
            {
                DrawFallback(position, label, idProperty, nameProperty);
                return;
            }

            SoundIdDropdownCache cache = GetOrCreateCache(type.Value);

            if (cache == null || !string.IsNullOrEmpty(cache.Error))
            {
                EditorGUI.HelpBox(
                    position,
                    cache?.Error ?? $"No enum found with [SoundIdEnum({type.Value})].",
                    MessageType.Error);
                return;
            }

            DrawEnumDropdown(position, label, cache, idProperty, nameProperty);
        }

        private SoundType? GetSoundKind()
        {
            SoundIdFieldAttribute attribute =
                fieldInfo.GetCustomAttribute<SoundIdFieldAttribute>();

            return attribute?.Type;
        }

        private static SoundIdDropdownCache GetOrCreateCache(SoundType soundType)
        {
            if (_cacheByType.TryGetValue(soundType, out SoundIdDropdownCache cache))
                return cache;

            Type[] enumTypes = FindSoundEnumTypes(soundType);

            if (enumTypes.Length == 0)
            {
                return new SoundIdDropdownCache
                {
                    Error = $"No enum found with [SoundIdEnum({soundType})]."
                };
            }

            if (enumTypes.Length > 1)
            {
                return new SoundIdDropdownCache
                {
                    Error = $"Multiple enums use [SoundIdEnum({soundType})]: " +
                            string.Join(", ", enumTypes.Select(type => type.FullName))
                };
            }

            Type enumType = enumTypes[0];

            string[] names = Enum.GetNames(enumType);
            Array values = Enum.GetValues(enumType);

            if (values.Length == 0)
            {
                return new SoundIdDropdownCache
                {
                    Error = $"{enumType.FullName} does not define any sound IDs."
                };
            }

            int[] ids = new int[values.Length];
            string[] displayOptions = new string[values.Length];
            HashSet<int> uniqueIds = new();

            for (int i = 0; i < values.Length; i++)
            {
                int id = Convert.ToInt32(values.GetValue(i));

                if (!uniqueIds.Add(id))
                {
                    return new SoundIdDropdownCache
                    {
                        Error = $"{enumType.FullName} contains duplicate sound ID {id}."
                    };
                }

                ids[i] = id;
                displayOptions[i] = $"{id} : {names[i]}";
            }

            cache = new SoundIdDropdownCache
            {
                EnumType = enumType,
                Names = names,
                Ids = ids,
                DisplayOptions = displayOptions
            };

            _cacheByType[soundType] = cache;
            return cache;
        }

        private static void DrawEnumDropdown(
            Rect position,
            GUIContent label,
            SoundIdDropdownCache cache,
            SerializedProperty idProperty,
            SerializedProperty nameProperty)
        {
            int currentIndex = GetCurrentIndex(cache, idProperty, nameProperty);

            int selectedIndex = EditorGUI.Popup(
                position,
                label.text,
                currentIndex,
                cache.DisplayOptions);

            if (selectedIndex < 0)
                return;

            idProperty.intValue = cache.Ids[selectedIndex];
            nameProperty.stringValue = cache.Names[selectedIndex];
        }

        private static int GetCurrentIndex(
            SoundIdDropdownCache cache,
            SerializedProperty idProperty,
            SerializedProperty nameProperty)
        {
            for (int i = 0; i < cache.Ids.Length; i++)
            {
                if (idProperty.intValue == cache.Ids[i] &&
                    nameProperty.stringValue == cache.Names[i])
                {
                    return i;
                }
            }

            return -1;
        }

        private static void DrawFallback(
            Rect position,
            GUIContent label,
            SerializedProperty idProperty,
            SerializedProperty nameProperty)
        {
            Rect idRect = new Rect(
                position.x,
                position.y,
                position.width * 0.35f,
                position.height);

            Rect nameRect = new Rect(
                position.x + position.width * 0.38f,
                position.y,
                position.width * 0.62f,
                position.height);

            EditorGUI.PropertyField(idRect, idProperty, GUIContent.none);
            EditorGUI.PropertyField(nameRect, nameProperty, label);
        }

        private static Type[] FindSoundEnumTypes(SoundType kind)
        {
            List<Type> matches = new();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic)
                    continue;

                Type[] types = GetTypesSafely(assembly);

                for (int i = 0; i < types.Length; i++)
                {
                    Type type = types[i];

                    if (type == null)
                        continue;

                    if (!type.IsEnum)
                        continue;

                    SoundIdEnumAttribute attribute =
                        type.GetCustomAttribute<SoundIdEnumAttribute>();

                    if (attribute == null)
                        continue;

                    if (attribute.Type == kind)
                        matches.Add(type);
                }
            }

            return matches
                .OrderBy(type => type.FullName)
                .ToArray();
        }

        [InitializeOnLoadMethod]
        private static void ClearCache()
        {
            _cacheByType.Clear();
        }

        private static Type[] GetTypesSafely(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                return exception.Types
                    .Where(type => type != null)
                    .ToArray();
            }
        }
    }
}

#endif
