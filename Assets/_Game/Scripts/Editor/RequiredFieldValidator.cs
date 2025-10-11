using System.Reflection;
using UnityEditor;
using UnityEngine;
using Core.Shared.Utilities;

public static class RequiredFieldValidator
{
    static RequiredFieldValidator()
    {
        EditorApplication.playModeStateChanged += (state) =>
        {
            if (state == PlayModeStateChange.ExitingEditMode)
                DebugWarnings();
        };
    }

    private static void DebugWarnings()
    {
        bool shouldPause = false;
        MonoBehaviour[] monoBehaviours = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (var _monoBehaviour in monoBehaviours)
        {
            FieldInfo[] fields = _monoBehaviour.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var _field in fields)
            {
                RequiredAttribute requiredAttribute = _field.GetCustomAttribute<RequiredAttribute>();

                if (requiredAttribute != null)
                {
                    object fieldValue = _field.GetValue(_monoBehaviour);

                    if (fieldValue.Equals(null))
                    {
                        Debug.LogError($"The field {_field.Name} is required!", _monoBehaviour);
                        shouldPause = true;
                    }
                }
            }
        }

        if (shouldPause)
            EditorApplication.ExitPlaymode();
    }
}