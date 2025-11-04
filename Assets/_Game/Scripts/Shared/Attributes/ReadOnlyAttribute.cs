using UnityEngine;

namespace Core.Shared.Attributes
{
    /// <summary>
    /// Makes a field read-only in the Unity Inspector but still serialized.
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
}