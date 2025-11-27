using Core.Events;
using Core.Shared;
using Core.Shared.Utilities;
using UnityEngine;

namespace Gameplay.FogOfWar 
{
    /// <summary>
    /// Contains position of renderer object and changes it's visibility.
    /// </summary>
    public class HiddenInFog : MonoBehaviour, IHiddenObject
    {
        [SerializeField, Required, Tooltip("MeshRenderer of this object, which has to be enabled or disabled.")] 
        private Renderer targetRenderer;


        private bool removed = false;


        /// <summary>
        /// Returns position of the revealer.
        /// </summary>
        /// <returns>World position of revealer.</returns>
        public Vector3 GetPosition()
        {
            return targetRenderer.transform.position;
        }


        /// <summary>
        /// Makes object visible or not.
        /// </summary>
        public void SetVisible(bool visible)
        {
            targetRenderer.enabled = visible;
        }


        /// <summary>
        /// Send message to fog of war manager to add this hidden in the fog object in runtime. Has to be called for spawned objects.
        /// </summary>
        public void DynamicallyAddHiddenObject()
        {
            EventManager.Broadcast(new AddHiddenObjectEvent(this));
            removed = false;
        }


        /// <summary>
        /// Send message to fog of war manager to remove this hidden in the fog object in runtime. Has to be called for despawned objects.
        /// </summary>
        public void DynamicallyRemoveHiddenObject()
        {
            if (removed) return;
            EventManager.Broadcast(new RemoveHiddenObjectEvent(this));
            removed = true;
        }


        private void OnDestroy()
        {
            if (!removed)
                DynamicallyRemoveHiddenObject();
        }
    }
}