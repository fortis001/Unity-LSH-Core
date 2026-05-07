#if UNITY_EDITOR

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LSH.Core.Editor
{
    [CustomEditor(typeof(CoreSceneSettings))]
    public class CoreSceneSettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty _sceneEnumTypeNameProperty;
        private SerializedProperty _entryCompleteSceneProperty;
        private SerializedProperty _loadingSceneProperty;
        private SerializedProperty _fallbackSceneProperty;

        private void OnEnable()
        {
            _sceneEnumTypeNameProperty = serializedObject.FindProperty("_sceneEnumTypeName");
            _entryCompleteSceneProperty = serializedObject.FindProperty("_entryCompleteScene");
            _loadingSceneProperty = serializedObject.FindProperty("_loadingScene");
            _fallbackSceneProperty = serializedObject.FindProperty("_fallbackScene");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Type[] enumTypes = FindSceneEnumTypes();

            if (enumTypes.Length == 0)
            {
                EditorGUILayout.HelpBox(
                    "No scene enum found. Create an enum and mark it with [SceneNameEnum].",
                    MessageType.Warning);

                DrawDefaultInspector();
                serializedObject.ApplyModifiedProperties();
                return;
            }

            DrawEnumTypeDropdown(enumTypes);

            Type selectedEnumType = enumTypes.FirstOrDefault(
                type => type.AssemblyQualifiedName == _sceneEnumTypeNameProperty.stringValue);

            if (selectedEnumType == null)
            {
                selectedEnumType = enumTypes[0];
                _sceneEnumTypeNameProperty.stringValue = selectedEnumType.AssemblyQualifiedName;
            }

            EditorGUILayout.Space(8);

            DrawSceneDropdown("Entry Complete Scene", _entryCompleteSceneProperty, selectedEnumType);
            DrawSceneDropdown("Loading Scene", _loadingSceneProperty, selectedEnumType);
            DrawSceneDropdown("Fallback Scene", _fallbackSceneProperty, selectedEnumType);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawEnumTypeDropdown(Type[] enumTypes)
        {
            string[] enumDisplayNames = enumTypes
                .Select(type => type.FullName)
                .ToArray();

            int currentIndex = Array.FindIndex(
                enumTypes,
                type => type.AssemblyQualifiedName == _sceneEnumTypeNameProperty.stringValue);

            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            int selectedIndex = EditorGUILayout.Popup(
                "Scene Enum Type",
                currentIndex,
                enumDisplayNames);

            _sceneEnumTypeNameProperty.stringValue = enumTypes[selectedIndex].AssemblyQualifiedName;
        }

        private void DrawSceneDropdown(
            string label,
            SerializedProperty sceneReferenceProperty,
            Type enumType)
        {
            SerializedProperty valueProperty = sceneReferenceProperty.FindPropertyRelative("_value");

            string[] enumNames = Enum.GetNames(enumType);

            string[] sceneNames = enumNames
                .Select(enumName => SceneNameUtility.From(enumType, enumName).ToString())
                .ToArray();

            int currentIndex = Array.IndexOf(sceneNames, valueProperty.stringValue);

            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            int selectedIndex = EditorGUILayout.Popup(label, currentIndex, sceneNames);

            valueProperty.stringValue = sceneNames[selectedIndex];
        }

        private static Type[] FindSceneEnumTypes()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(assembly => !assembly.IsDynamic)
                .SelectMany(GetTypesSafely)
                .Where(type =>
                    type.IsEnum &&
                    type.GetCustomAttribute<SceneNameEnumAttribute>() != null)
                .OrderBy(type => type.FullName)
                .ToArray();
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