#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Core.Shared.Attributes.Editor
{
    /// <summary>
    /// Custom property drawer for ReadOnly attribute that disables the field in inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Store old GUI enabled value
            bool previousGUIState = GUI.enabled;
            
            // Disable the field
            GUI.enabled = false;
            
            // Draw field as usual
            EditorGUI.PropertyField(position, property, label);
            
            // Restore GUI state
            GUI.enabled = previousGUIState;
        }
    }
}
#endif