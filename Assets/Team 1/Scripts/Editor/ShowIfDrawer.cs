using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CustomEditor.Attributes
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIf = (ShowIfAttribute)attribute;

            object target = property.serializedObject.targetObject;


            bool show = false;

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            MethodInfo method = target.GetType().GetMethod(showIf.ConditionName, flags);

            if (method != null && method.ReflectedType == typeof(bool))
            {
                show = (bool)method.Invoke(target, null);
            }
            else
            {
                FieldInfo field = target.GetType().GetField(showIf.ConditionName, flags);

                if (field != null && field.FieldType == typeof(bool))
                {
                    show = (bool)field.GetValue(target);
                }
                else
                {
                    PropertyInfo prop = target.GetType().GetProperty(showIf.ConditionName, flags);

                    if (prop != null && prop.PropertyType == typeof(bool))
                    {
                        show = (bool)prop.GetValue(target);
                    }
                }
            }

            if (showIf.Invert)
                show = !show;

            if (show)
                EditorGUI.PropertyField(position, property, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIf = (ShowIfAttribute)attribute;

            object target = property.serializedObject.targetObject;


            bool show = false;

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            MethodInfo method = target.GetType().GetMethod(showIf.ConditionName, flags);

            if (method != null && method.ReflectedType == typeof(bool))
            {
                show = (bool)method.Invoke(target, null);
            }
            else
            {
                FieldInfo field = target.GetType().GetField(showIf.ConditionName, flags);

                if (field != null && field.FieldType == typeof(bool))
                {
                    show = (bool)field.GetValue(target);
                }
                else
                {
                    PropertyInfo prop = target.GetType().GetProperty(showIf.ConditionName, flags);

                    if (prop != null && prop.PropertyType == typeof(bool))
                    {
                        show = (bool)prop.GetValue(target);
                    }
                }
            }

            if (showIf.Invert)
                show = !show;

            return show ? EditorGUI.GetPropertyHeight(property, true) : 0;
        }
    }
}