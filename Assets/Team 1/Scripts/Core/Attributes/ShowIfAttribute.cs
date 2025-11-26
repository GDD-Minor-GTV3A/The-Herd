using UnityEngine;

namespace CustomEditor.Attributes 
{
    /// <summary>
    /// Attribute can be used to show or hide serialized fields in Inspector depending on condition.
    /// </summary>
    public class ShowIfAttribute : PropertyAttribute
    {
        /// <summary>
        /// Name of condition. Condition can be field, method, property.
        /// </summary>
        public string ConditionName;
        /// <summary>
        /// If true condition will be inversed.
        /// </summary>
        public bool Invert;

        public ShowIfAttribute(string conditionName, bool invert = false)
        {
            ConditionName = conditionName;
            Invert = invert;
        }
    }
}