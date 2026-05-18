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

            if (cache == null || cache.EnumType == null)
            {
                EditorGUI.HelpBox(
                    position,
                    $"No enum found with [SoundIdEnum({type.Value})].",
                    MessageType.Warning);
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

            Type enumType = FindSoundEnumType(soundType);

            if (enumType == null)
                return null;

            string[] names = Enum.GetNames(enumType);
            Array values = Enum.GetValues(enumType);

            int[] ids = new int[values.Length];
            string[] displayOptions = new string[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                int id = Convert.ToInt32(values.GetValue(i));

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

            _cacheByType.Add(soundType, cache);
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

            return 0;
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

        private static Type FindSoundEnumType(SoundType kind)
        {
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
                        return type;
                }
            }

            return null;
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