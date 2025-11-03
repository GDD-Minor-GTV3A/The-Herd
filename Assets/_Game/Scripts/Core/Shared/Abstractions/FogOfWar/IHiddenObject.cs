using UnityEngine;

namespace Core.Shared 
{
    public interface IHiddenObject
    {

        /// <summary>
        /// Returns position of renderer object object.
        /// </summary>
        /// <returns>Position of renderer.</returns>
        public abstract Vector3 GetPosition();

        /// <summary>
        /// Sets visibility of this object.
        /// </summary>
        /// <param name="visible">True - visible, false - not visible.</param>
        public abstract void SetVisible(bool visible);


        /// <summary>
        /// Dynamically adds hidden object to fog of war.
        /// </summary>
        public abstract void DynamicallyAddHiddenObject();

        /// <summary>
        /// Dynamically removes hidden object from fog of war.
        /// </summary>
        public abstract void DynamicallyRemoveHiddenObject();

    }
}