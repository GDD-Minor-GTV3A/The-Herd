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
        [SerializeField, Required, Tooltip("MeshRenderer of this object, which has to be enabled or disabled.")] private Renderer targetRenderer;


        private bool removed = false;


        public Vector3 GetPosition()
        {
            return targetRenderer.transform.position;
        }


        public void SetVisible(bool visible)
        {
            targetRenderer.enabled = visible;
        }


        public void DynamicallyAddHiddenObject()
        {
            EventManager.Broadcast(new AddHiddenObjectEvent(this));
            removed = false;
        }


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