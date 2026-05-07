#if UNITY_EDITOR

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LSH.Core.Editor
{
    [CustomPropertyDrawer(typeof(SoundId))]
    public class SoundIdDrawer : PropertyDrawer
    {
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

            Type enumType = FindSoundEnumType(type.Value);

            if (enumType == null)
            {
                EditorGUI.HelpBox(
                    position,
                    $"No enum found with [SoundIdEnum(SoundKind.{type.Value})].",
                    MessageType.Warning);
                return;
            }

            DrawEnumDropdown(position, label, enumType, idProperty, nameProperty);
        }

        private SoundType? GetSoundKind()
        {
            SoundIdFieldAttribute attribute =
                fieldInfo.GetCustomAttribute<SoundIdFieldAttribute>();

            return attribute?.Type;
        }

        private static void DrawEnumDropdown(
            Rect position,
            GUIContent label,
            Type enumType,
            SerializedProperty idProperty,
            SerializedProperty nameProperty)
        {
            string[] names = Enum.GetNames(enumType);
            Array values = Enum.GetValues(enumType);

            string[] displayOptions = names
                .Select((name, index) =>
                {
                    int id = Convert.ToInt32(values.GetValue(index));
                    return $"{id} : {name}";
                })
                .ToArray();

            int currentIndex = -1;

            for (int i = 0; i < values.Length; i++)
            {
                int id = Convert.ToInt32(values.GetValue(i));
                string name = names[i];

                if (idProperty.intValue == id && nameProperty.stringValue == name)
                {
                    currentIndex = i;
                    break;
                }
            }

            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            int selectedIndex = EditorGUI.Popup(
                position,
                label.text,
                currentIndex,
                displayOptions);

            int selectedId = Convert.ToInt32(values.GetValue(selectedIndex));
            string selectedName = names[selectedIndex];

            idProperty.intValue = selectedId;
            nameProperty.stringValue = selectedName;
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
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(assembly => !assembly.IsDynamic)
                .SelectMany(GetTypesSafely)
                .Where(type =>
                    type.IsEnum &&
                    type.GetCustomAttribute<SoundIdEnumAttribute>()?.Type == kind)
                .OrderBy(type => type.FullName)
                .FirstOrDefault();
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