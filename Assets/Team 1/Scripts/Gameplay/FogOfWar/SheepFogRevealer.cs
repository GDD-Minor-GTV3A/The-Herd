using System.Collections.Generic;
using Core.Events;
using UnityEngine;

namespace Gameplay.FogOfWar
{
    public class SheepFogRevealer : FogRevealer
    {
        private List<AnimationCurve> curvesList;
        private List<float> viewDistanceMultiplayers;
        private Vector3 playerPosition;


        public override void Initialize(Transform fogPlane, Material meshMaterial, LayerMask obstaclesLayers)
        {

            base.Initialize(fogPlane, meshMaterial, obstaclesLayers);

            curvesList = new List<AnimationCurve>();
            viewDistanceMultiplayers = new List<float>();

            foreach (Revealer revealer in revealers)
            {
                AnimationCurve curve = (revealer.Config as SheepFogRevealerConfig).DistanceFadingCurve;

                curvesList.Add(curve);
                viewDistanceMultiplayers.Add(1);
            }

            EventManager.AddListener<PlayerSquareTickEvent>(UpdatePlayerPosition);
        }


        private void UpdatePlayerPosition(PlayerSquareTickEvent evt)
        {
            playerPosition = evt.Center;
        }


        protected override float GetRevealerDistance(int index)
        {
            if (curvesList == null || viewDistanceMultiplayers == null)
                return base.GetRevealerDistance(index);

            float distanceToPlayer = Vector3.Distance(origin.position, playerPosition);

            Debug.Log(distanceToPlayer);

            float newMultiplayer = curvesList[index].Evaluate(distanceToPlayer);

            if (newMultiplayer != viewDistanceMultiplayers[index])
                ChangeViewDistanceMultiplayer(index, newMultiplayer);



            return revealers[index].Config.ViewDistance * viewDistanceMultiplayers[index];
        }


        private void ChangeViewDistanceMultiplayer(int index, float newMultiplayer)
        {
            viewDistanceMultiplayers[index] = newMultiplayer;

            revealers[index].Renderer.material.SetFloat("_ViewDistance", revealers[index].Config.ViewDistance * newMultiplayer);
        }
    }
}
