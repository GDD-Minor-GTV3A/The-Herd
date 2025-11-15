using UnityEngine;


namespace CustomEditor.Attributes 
{
    public class ShowIfAttribute : PropertyAttribute
    {
        public string ConditionName;
        public bool Invert;

        public ShowIfAttribute(string conditionName, bool invert = false)
        {
            ConditionName = conditionName;
            Invert = invert;
        }
    }
}

