using System.Collections;
using Core.Shared.Utilities;
using UnityEngine;

namespace Gameplay.FogOfWar 
{
    public class HiddenInFog : MonoBehaviour
    {
        [SerializeField, Required, Tooltip("")] private MeshRenderer renderer;


        public Vector3 GetPosition()
        {
            return renderer.transform.position;
        }


        public void SetVisible(bool visible)
        {
            renderer.enabled = visible;
        }
    }
}