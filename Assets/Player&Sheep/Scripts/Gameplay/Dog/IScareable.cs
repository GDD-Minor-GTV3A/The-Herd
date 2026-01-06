using UnityEngine;

namespace Gameplay.Dog
{
    /// <summary>
    /// Interface for entities which can be scared.
    /// </summary>
    public interface IScareable
    {
        /// <summary>
        /// Invokes when object scared.
        /// </summary>
        /// <param name="fromPosition">Position of the dog when it scared entity.</param>
        /// <param name="intensity">Intensity of scare.</param>
        /// <param name="scareType">Type of scare.</param>
        public void OnScared(Vector3 fromPosition, float intensity, ScareType scareType);
    }
}


/// <summary>
/// Different types of scare.
/// </summary>
public enum ScareType
{
    DogBark,
    Growl,
    Explosion,
    Gunshot,
    Thunder,
    Other
}