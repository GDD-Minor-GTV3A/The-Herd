using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gameplay.FogOfWar
{
    public class FogRevealer : MonoBehaviour
    {
        [SerializeField] private List<Revealer> _revealers;

        private List<GameObject> _fovMeshObjects;
        private List<Mesh> _fovMeshes;
        private List<float> _startingAngles;
        private LayerMask _obstaclesLayers;


        public void CreateFovMeshes(Transform fogPlane, Material meshMaterial, LayerMask obstaclesLayers)
        {
            _fovMeshObjects = new List<GameObject>();
            _fovMeshes = new List<Mesh>();
            _startingAngles = new List<float>();

            int i = 0;
            foreach (Revealer revealer in _revealers)
            {
                Mesh newMesh = new Mesh();

                GameObject newFovMeshObject = new GameObject();
                newFovMeshObject.name = "FovMesh";
                newFovMeshObject.transform.position = new Vector3(revealer.Origin.position.x, fogPlane.transform.position.y + 1, revealer.Origin.position.z);
                newFovMeshObject.transform.parent = fogPlane.transform.parent;
                newFovMeshObject.layer = LayerMask.NameToLayer("FogOfWarProjection");
                newFovMeshObject.AddComponent<MeshFilter>().mesh = newMesh;
                newFovMeshObject.AddComponent<MeshRenderer>();

                _fovMeshObjects.Add(newFovMeshObject);
                _fovMeshes.Add(newMesh);
                _startingAngles.Add(0);
                UpdateMesh(i);
                StartCoroutine(UpdateMeshCor(revealer.Config.UpdateRate, i));
                i++;
            }

            
            UpdateRevealerMaterial(meshMaterial);

            UpdateObstaclesMask(obstaclesLayers);

        }


        private void Update()
        {
            if (_fovMeshes == null) return;
            for (int i = 0; i < _fovMeshObjects.Count; i++)
            {
                Transform origin = _revealers[i].Origin;

                _fovMeshObjects[i].transform.position = new Vector3(origin.position.x, _fovMeshObjects[i].transform.position.y, origin.position.z);

                Vector3 direction = origin.forward;
                direction = direction.normalized;
                float result = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
                result += 150f;

                if (result < 0) result += 360;

                _startingAngles[i] = result - _revealers[i].Config.FOV / 2f;

                _fovMeshObjects[i].GetComponent<MeshRenderer>().material.SetVector("_RevealerCenter", _fovMeshObjects[i].transform.position);
                _fovMeshObjects[i].GetComponent<MeshRenderer>().material.SetVector("_RevealerForward", _revealers[i].Origin.forward);
                _fovMeshObjects[i].GetComponent<MeshRenderer>().material.SetFloat("_ViewDistance", _revealers[i].Config.ViewDistance);
                _fovMeshObjects[i].GetComponent<MeshRenderer>().material.SetFloat("_FOVAngle", _revealers[i].Config.FOV * 0.5f * Mathf.Deg2Rad);
            }

        }


        private void UpdateMesh(int meshIndex)
        {
            float fov = _revealers[meshIndex].Config.FOV;
            float viewDistance = _revealers[meshIndex].Config.ViewDistance;
            int rayCount = (int)_revealers[meshIndex].Config.RayCount;
            Transform origin = _revealers[meshIndex].Origin;

            float angle = _startingAngles[meshIndex];
            float angleIncrease = fov / rayCount;
            Vector3 meshOrigin = Vector3.zero;

            Vector3[] vertices = new Vector3[rayCount + 1 + 1];
            int[] triangles = new int[rayCount * 3];

            vertices[0] = meshOrigin;

            int vertexIndex = 1;
            int triangleIndex = 0;

            for (int v = 0; v <= rayCount; v++)
            {
                float angleRad = angle * (Mathf.PI / 180f);
                Vector3 result = new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
                Vector3 vertex;
                if (Physics.SphereCast(origin.position, .1f, result, out RaycastHit hit, viewDistance, _obstaclesLayers))
                {
                    Vector3 localHitPoint = _fovMeshObjects[meshIndex].transform.InverseTransformPoint(hit.point + result * .5f);
                    vertex = new Vector3(localHitPoint.x, meshOrigin.y, localHitPoint.z);
                }
                else
                {
                    vertex = meshOrigin + result * viewDistance;
                }

                vertices[vertexIndex] = vertex;

                if (v > 0)
                {
                    triangles[triangleIndex] = 0;
                    triangles[triangleIndex + 1] = vertexIndex - 1;
                    triangles[triangleIndex + 2] = vertexIndex;
                    triangleIndex += 3;
                }

                angle -= angleIncrease;
                vertexIndex++;
            }

            _fovMeshes[meshIndex].vertices = vertices;
            _fovMeshes[meshIndex].triangles = triangles;
        }


        public void UpdateObstaclesMask(LayerMask obstaclesLayers)
        {
            _obstaclesLayers = obstaclesLayers;
        }

        public void UpdateRevealerMaterial(Material revealerMaterial)
        {
            if (_fovMeshObjects != null)
                foreach (GameObject fovObject in _fovMeshObjects)
                    fovObject.GetComponent<MeshRenderer>().material = revealerMaterial;
        }


        [Serializable]
        private struct Revealer
        {
            public FogRevealerConfig Config;
            public Transform Origin;
        }


        private IEnumerator UpdateMeshCor(float updateTime, int meshIndex)
        {
            yield return null;
            while (true)
            {
                yield return new WaitForSeconds(updateTime);
                UpdateMesh(meshIndex);
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}