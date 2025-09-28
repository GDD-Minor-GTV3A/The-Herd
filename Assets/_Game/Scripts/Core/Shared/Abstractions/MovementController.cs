using UnityEngine;

namespace Core.Shared
{
    /// <summary>
    /// Use this class as a base for movement controllers.
    /// </summary>
    public abstract class MovementController : MonoBehaviour
    {
        /// <summary>
        /// Moves object to target position.
        /// </summary>
        /// <param name="target">Target position.</param>
        public abstract void MoveTo(Vector3 target);
    }
}