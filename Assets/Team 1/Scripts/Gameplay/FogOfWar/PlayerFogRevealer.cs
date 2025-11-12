
using System;

using Core.Events;

using UnityEngine;

namespace Gameplay.FogOfWar
{
    public class PlayerFogRevealer : FogRevealer
    {
        private int coneIndex = -1;
        private float coneFOVChangeValue = 0;
        private float coneDistanceChangeValue = 0;


        public override void Initialize(Transform fogPlane, Material meshMaterial, LayerMask obstaclesLayers)
        {
            EventManager.AddListener<ChangeConePlayerRevealerFOVEvent>(ChangeConeFOV);
            EventManager.AddListener<ChangeConePlayerRevealerDistnaceEvent>(ChangeConeDistance);
            base.Initialize(fogPlane, meshMaterial, obstaclesLayers);
        }

        

        protected override void CreateNewMesh(int revealerIndex, Transform fogPlane, Material meshMaterial)
        {
            if (revealers[revealerIndex].Config.FOV < 360) coneIndex = revealerIndex;
            base.CreateNewMesh(revealerIndex, fogPlane, meshMaterial);
        }


        private void ChangeConeDistance(ChangeConePlayerRevealerDistnaceEvent evt)
        {
            coneDistanceChangeValue += evt.Value;

            revealers[coneIndex].Renderer.material.SetFloat("_ViewDistance", GetRevealerDistance(coneIndex));
        }

        private void ChangeConeFOV(ChangeConePlayerRevealerFOVEvent evt)
        {
            coneFOVChangeValue += evt.Value;

            revealers[coneIndex].Renderer.material.SetFloat("_FOVAngle", GetRevealerFOV(coneIndex) * 0.5f * Mathf.Deg2Rad);
            revealers[coneIndex].Renderer.material.SetFloat("_EdgeFadeWidth", GetRevealerFOV(coneIndex) / 250);
        }

        protected override float GetRevealerFOV(int index)
        {
            float fov = revealers[index].Config.FOV;
            fov += (index == coneIndex) ? coneFOVChangeValue : 0;

            return fov;
        }

        protected override float GetRevealerDistance(int index)
        {
            float distance = revealers[index].Config.ViewDistance;
            distance += (index == coneIndex) ? coneDistanceChangeValue : 0;

            return distance;
        }


        protected override void OnDestroy()
        {
            EventManager.RemoveListener<ChangeConePlayerRevealerFOVEvent>(ChangeConeFOV);
            EventManager.RemoveListener<ChangeConePlayerRevealerDistnaceEvent>(ChangeConeDistance);
            base.OnDestroy();
        }
    }
}
