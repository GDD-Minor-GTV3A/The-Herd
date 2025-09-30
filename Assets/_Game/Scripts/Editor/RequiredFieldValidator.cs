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
        bool _shouldPause = false;
        MonoBehaviour[] _monoBehaviours = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (var monoBehaviour in _monoBehaviours)
        {
            FieldInfo[] _fields = monoBehaviour.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in _fields)
            {
                RequiredAttribute _requiredAttribute = field.GetCustomAttribute<RequiredAttribute>();

                if (_requiredAttribute != null)
                {
                    object _fieldValue = field.GetValue(monoBehaviour);

                    if (_fieldValue.Equals(null))
                    {
                        Debug.LogError($"The field {field.Name} is required!", monoBehaviour);
                        _shouldPause = true;
                    }
                }
            }
        }

        if (_shouldPause)
            EditorApplication.ExitPlaymode();
    }
}
