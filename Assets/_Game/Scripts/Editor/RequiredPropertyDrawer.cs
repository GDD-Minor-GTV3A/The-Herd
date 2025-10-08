using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Core.Shared.Utilities.RequiredAttribute))]
public class RequiredPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label);

        if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
        {
            EditorGUILayout.HelpBox("This field is required!", MessageType.Error);
        }
    }
}